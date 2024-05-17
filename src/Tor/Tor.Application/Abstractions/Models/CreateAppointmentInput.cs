using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;

namespace Tor.Application.Abstractions.Models;

public record CreateAppointmentInput(
    Guid StaffMemberId,
    Guid? ClientId,
    DateTimeOffset ScheduledFor,
    AppointmentType Type,
    ClientDetails ClientDetails,
    ServiceDetails ServiceDetails,
    string? Notes,
    int AppointmentReminderTimeInHours);
