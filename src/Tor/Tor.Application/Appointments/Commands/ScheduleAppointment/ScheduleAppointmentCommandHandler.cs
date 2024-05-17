using FluentResults;
using Medallion.Threading;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Common;
using Tor.Application.Appointments.Notifications.AppointmentScheduled;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Common.Extensions;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Appointments.Commands.ScheduleAppointment;

internal sealed class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, Result<Appointment>>
{
    private readonly ITorDbContext _context;
    private readonly IAppointmentRepository _repository;
    private readonly IPublisher _publisher;
    private readonly IAvailableTimesCalculator _availableTimesCalculator;
    private readonly IDistributedLockProvider _lockProvider;

    public ScheduleAppointmentCommandHandler(
        ITorDbContext context,
        IAppointmentRepository repository,
        IPublisher publisher,
        IAvailableTimesCalculator availableTimesCalculator,
        IDistributedLockProvider lockProvider)
    {
        _context = context;
        _repository = repository;
        _publisher = publisher;
        _availableTimesCalculator = availableTimesCalculator;
        _lockProvider = lockProvider;
    }

    public async Task<Result<Appointment>> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        StaffMember? staffMember = await _context.StaffMembers
            .AsNoTracking()
            .Include(x => x.Business)
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staffMember is null)
        {
            return Result.Fail<Appointment>(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        if (request.ClientId is not null &&
            !(await _context.Clients.AnyAsync(x => x.Id == request.ClientId, cancellationToken)))
        {
            return Result.Fail<Appointment>(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        bool isClientBlocked = await _context.BlockedClients
            .AnyAsync(x => x.BusinessId == staffMember.BusinessId && x.ClientId == request.ClientId, cancellationToken);
        if (isClientBlocked)
        {
            return Result.Fail<Appointment>(
                new DomainError("client is blocked! cant schedule appointment"));
        }

        Result<ServiceDetails> serviceDetailsResult = await GetServiceDetails(request, cancellationToken);
        if (serviceDetailsResult.IsFailed)
        {
            return Result.Fail<Appointment>(serviceDetailsResult.Errors);
        }
        ServiceDetails serviceDetails = serviceDetailsResult.Value;

        if (request.ClientId is not null &&
            await IsExceededMaximumAppointments(request.ClientId!.Value, staffMember.BusinessId, staffMember.Business.Settings.MaximumAppointmentsForClient, cancellationToken))
        {
            return Result.Fail<Appointment>(
                new CustomError(Constants.ErrorMessages.ExceededMaximumAppointments));
        }

        CreateAppointmentInput input = new(
            request.StaffMemberId,
            request.ClientId,
            request.ScheduledFor,
            request.Type,
            request.ClientDetails,
            serviceDetails,
            request.Notes,
            staffMember.Business.Settings.AppointmentReminderTimeInHours);

        if (request.Type == AppointmentType.Manual)
        {
            return Result.Ok(await _repository.Create(input, cancellationToken));
        }

        Result<Appointment> appointmentResult = await TryCreateAppointment(
            staffMember.Business.Settings,
            staffMember.WeeklySchedule,
            staffMember.Id,
            TimeSpan.FromMinutes(serviceDetails.Durations.First().ValueInMinutes),
            input,
            cancellationToken);

        if (appointmentResult.IsFailed)
        {
            return appointmentResult;
        }

        await AppointmentScheduled(request, staffMember, appointmentResult.Value.ScheduledFor, cancellationToken);

        return Result.Ok(appointmentResult.Value);
    }

    private async Task AppointmentScheduled(
        ScheduleAppointmentCommand request,
        StaffMember staffMember,
        DateTimeOffset scheduledFor,
        CancellationToken cancellationToken)
    {
        AppointmentScheduledNotification notification = new(
            request.ClientId,
            staffMember.Id,
            request.ClientDetails.Name,
            staffMember.BusinessId,
            scheduledFor,
            request.NotifyClient,
            staffMember.Settings.SendNotificationsWhenAppointmentScheduled);

        await _publisher.Publish(notification, cancellationToken);
    }

    private async Task<Result<ServiceDetails>> GetServiceDetails(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (request.ServiceId is null)
        {
            return request.ServiceDetails;
        }

        Service? service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == request.ServiceId, cancellationToken);

        if (service is null)
        {
            return Result.Fail<ServiceDetails>(
                new NotFoundError($"could not find service with id {request.ServiceId}"));
        }

        return new ServiceDetails
        {
            Name = service.Name,
            Amount = service.Amount,
            Description = service.Description,
            Durations = service.Durations,
        };
    }

    private async Task<Result<Appointment>> TryCreateAppointment(
        BusinessSettings settings,
        WeeklySchedule weeklySchedule,
        Guid staffMemberId,
        TimeSpan duration,
        CreateAppointmentInput input,
        CancellationToken cancellationToken)
    {
        IDistributedLock @lock = _lockProvider.CreateLock($"ScheduleAppointment:{staffMemberId}:{DateOnly.FromDateTime(input.ScheduledFor.UtcDateTime)}");
        await using var handle = await @lock.TryAcquireAsync(TimeSpan.FromSeconds(5), cancellationToken);

        if (handle is null)
        {
            return Result.Fail<Appointment>(
                new DomainError($"could not acquire distributed lock {@lock.Name}"));
        }

        return !await CanScheduleAppointment(input.ScheduledFor, duration, settings, weeklySchedule, staffMemberId, cancellationToken)
            ? Result.Fail<Appointment>(new CustomError(Constants.ErrorMessages.AppointmentTaken))
            : await _repository.Create(input, cancellationToken);
    }

    private async Task<bool> IsExceededMaximumAppointments(
        Guid clientId,
        Guid businessId,
        int maximumAppointmentsForClient,
        CancellationToken cancellationToken)
    {
        int numberOfAppointments = await _context.Appointments
            .Where(x => x.ClientId == clientId)
            .Where(x => x.StaffMember.BusinessId == businessId)
            .Where(x => x.Status != AppointmentStatusType.Canceled)
            .Where(x => x.ScheduledFor >= DateTimeOffset.UtcNow)
            .CountAsync(cancellationToken);

        return numberOfAppointments >= maximumAppointmentsForClient;
    }

    private async Task<bool> CanScheduleAppointment(
        DateTimeOffset scheduledFor,
        TimeSpan duration,
        BusinessSettings settings,
        WeeklySchedule weeklySchedule,
        Guid staffMemberId,
        CancellationToken cancellationToken)
    {
        DateTime scheduledForInIst = scheduledFor.ToIsraelTime();

        var availableTimes = await _availableTimesCalculator.CalculateAvailableTimes(
            DateOnly.FromDateTime(scheduledForInIst),
            duration,
            settings,
            weeklySchedule,
            staffMemberId,
            cancellationToken);

        return availableTimes.Any(x =>
            x.StartTime >= TimeOnly.FromDateTime(scheduledForInIst) &&
            x.EndTime <= TimeOnly.FromDateTime(scheduledForInIst + duration));
    }
}
