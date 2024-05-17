using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;

namespace Tor.Contracts.Business;

public class GetAppointmentsByClientResponse
{
    public List<AppointmentByClientResponse> Appointments { get; set; } = [];
}

public class AppointmentByClientResponse
{
    public Guid Id { get; set; }
    public string StaffMemberName { get; set; } = string.Empty;
    public AppointmentType Type { get; set; }
    public AppointmentStatusType Status { get; set; }
    public DateTimeOffset ScheduledFor { get; set; }
    public Guid? ServiceId { get; set; } = null!;
    public ServiceDetails ServiceDetails { get; set; } = default!;
    public ClientDetails ClientDetails { get; set; } = default!;
    public bool HasReceivedReminder { get; set; }
}
