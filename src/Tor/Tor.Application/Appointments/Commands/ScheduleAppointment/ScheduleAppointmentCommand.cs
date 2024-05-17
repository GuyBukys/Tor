using FluentResults;
using MediatR;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;

namespace Tor.Application.Appointments.Commands.ScheduleAppointment;

public record ScheduleAppointmentCommand(
    Guid StaffMemberId,
    Guid? ClientId,
    AppointmentType Type,
    DateTimeOffset ScheduledFor,
    ClientDetails ClientDetails,
    Guid? ServiceId,
    ServiceDetails ServiceDetails,
    string? Notes,
    bool NotifyClient = true) : IRequest<Result<Appointment>>;
