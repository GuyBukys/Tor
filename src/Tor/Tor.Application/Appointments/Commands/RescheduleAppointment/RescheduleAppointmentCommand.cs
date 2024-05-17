using FluentResults;
using MediatR;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.ValueObjects;

namespace Tor.Application.Appointments.Commands.RescheduleAppointment;

public record RescheduleAppointmentCommand(
    Guid AppointmentId,
    Guid StaffMemberId,
    Guid? ClientId,
    DateTimeOffset ScheduledFor,
    ClientDetails ClientDetails,
    Guid? ServiceId,
    ServiceDetails ServiceDetails,
    string? Notes,
    bool NotifyClient = false) : IRequest<Result<Appointment>>;
