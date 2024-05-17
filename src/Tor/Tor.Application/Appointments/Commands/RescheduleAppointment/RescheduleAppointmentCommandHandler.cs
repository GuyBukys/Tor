using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Notifications.AppointmentScheduled;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Appointments.Commands.RescheduleAppointment;

internal sealed class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Result<Appointment>>
{
    private readonly ITorDbContext _context;
    private readonly IAppointmentRepository _repository;
    private readonly IPublisher _publisher;

    public RescheduleAppointmentCommandHandler(
        ITorDbContext context,
        IAppointmentRepository repository,
        IPublisher publisher)
    {
        _context = context;
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result<Appointment>> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        bool isAppointmnetExist = await _context.Appointments
            .AnyAsync(x => x.Id == request.AppointmentId, cancellationToken);

        if (!isAppointmnetExist)
        {
            return Result.Fail(
                new NotFoundError($"could not find appointment with id {request.AppointmentId}"));
        }

        Result<ServiceDetails> serviceDetailsResult = await GetServiceDetails(request, cancellationToken);
        if (serviceDetailsResult.IsFailed)
        {
            return Result.Fail<Appointment>(serviceDetailsResult.Errors);
        }
        ServiceDetails serviceDetails = serviceDetailsResult.Value;

        BusinessSettings settings = await _context.StaffMembers
            .AsNoTracking()
            .Where(x => x.Id == request.StaffMemberId)
            .Select(x => x.Business.Settings)
            .FirstAsync(cancellationToken);

        using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        await _repository.Cancel(request.AppointmentId, cancellationToken);

        CreateAppointmentInput input = new(
            request.StaffMemberId,
            request.ClientId,
            request.ScheduledFor,
            AppointmentType.Manual,
            request.ClientDetails,
            serviceDetails,
            request.Notes,
            settings.AppointmentReminderTimeInHours);

        Appointment newAppointment = await _repository.Create(input, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        await AppointmentScheduled(newAppointment, request.NotifyClient, cancellationToken);

        return newAppointment;
    }

    private async Task<Result<ServiceDetails>> GetServiceDetails(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
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

    private async Task AppointmentScheduled(Appointment appointment, bool notifyClient, CancellationToken cancellationToken)
    {
        Guid businessId = await _context.StaffMembers
            .Where(x => x.Id == appointment.StaffMemberId)
            .Select(x => x.BusinessId)
            .FirstAsync(cancellationToken);

        var notification = new AppointmentScheduledNotification(
            appointment.ClientId,
            appointment.StaffMemberId,
            appointment.ClientDetails.Name,
            businessId,
            appointment.ScheduledFor,
            notifyClient,
            false);

        await _publisher.Publish(notification, cancellationToken);
    }
}
