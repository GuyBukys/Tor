using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Clients.Queries.GetAppointments;

public record ClientAppointmentResult(
    Guid Id,
    string StaffMemberName,
    AppointmentType Type,
    AppointmentStatusType Status,
    DateTimeOffset ScheduledFor,
    ServiceDetails ServiceDetails,
    BusinessDetails BusinessDetails,
    bool IsCancellable);

public record BusinessDetails(
    Guid Id,
    string Name,
    Image? Logo,
    Image? Cover,
    Address? Address,
    PhoneNumber PhoneNumber);