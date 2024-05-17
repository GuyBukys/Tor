using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Appointments.Notifications.AppointmentCanceled;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.AppointmentAggregate;

namespace Tor.Application.Appointments.Commands.CancelAppointment;

internal sealed class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IAppointmentRepository _repository;
    private readonly IPublisher _publisher;

    public CancelAppointmentCommandHandler(
        ITorDbContext context,
        IAppointmentRepository repository,
        IPublisher publisher)
    {
        _context = context;
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        Appointment? appointment = await _context.Appointments
            .Include(x => x.StaffMember)
            .FirstOrDefaultAsync(x => x.Id == request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find appointment with id {request.AppointmentId}"));
        }

        bool canCancelAppointment = request.ForceCancel || await CanCancelAppointment(appointment, cancellationToken);
        if (!canCancelAppointment)
        {
            return Result.Fail(
                new CustomError(Constants.ErrorMessages.CannotCancelAppointment));
        }

        await _repository.Cancel(request.AppointmentId, cancellationToken);

        var notification = new AppointmentCanceledNotification(
            appointment.StaffMember.BusinessId,
            appointment.StaffMemberId,
            appointment.ServiceDetails.Name,
            appointment.ClientId,
            appointment.ClientDetails.Name,
            appointment.ScheduledFor,
            request.NotifyWaitingList,
            request.NotifyClient,
            appointment.StaffMember.Settings.SendNotificationsWhenAppointmentCanceled,
            request.Reason);

        await _publisher.Publish(notification, cancellationToken);

        return Result.Ok();
    }

    private async Task<bool> CanCancelAppointment(Appointment appointment, CancellationToken cancellationToken)
    {
        int cancelAppointmentMinimumTimeInMinutes = await _context.Businesses
            .Where(x => x.Id == appointment.StaffMember.BusinessId)
            .Select(x => x.Settings.CancelAppointmentMinimumTimeInMinutes)
            .FirstAsync(cancellationToken);

        return DateTime.UtcNow.AddMinutes(cancelAppointmentMinimumTimeInMinutes) <= appointment.ScheduledFor;
    }
}
