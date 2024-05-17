using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;

namespace Tor.Contracts.StaffMember;

public class StaffMemberAppointmentResponse
{
    public Guid Id { get; set; }
    public AppointmentType Type { get; set; }
    public AppointmentStatusType Status { get; set; }
    public DateTimeOffset ScheduledFor { get; set; }
    public ClientDetails ClientDetails { get; set; } = default!;
    public Guid? ServiceId { get; set; } = null!;
    public ServiceDetails ServiceDetails { get; set; } = default!;
    public bool HasReceivedReminder { get; set; }
}
