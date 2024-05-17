using FluentResults;
using MediatR;

namespace Tor.Application.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(
    Guid AppointmentId,
    string? Reason,
    bool ForceCancel = false,
    bool NotifyClient = true,
    bool NotifyWaitingList = true) : IRequest<Result>;