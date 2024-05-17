using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.Client;

public class ClientAppointmentResponse
{
    public Guid Id { get; set; }
    public string StaffMemberName { get; set; } = string.Empty;
    public AppointmentType Type { get; set; }
    public AppointmentStatusType Status { get; set; }
    public DateTimeOffset ScheduledFor { get; set; }
    public ServiceDetails ServiceDetails { get; set; } = default!;
    public BusinessDetailsResponse BusinessDetails { get; set; } = default!;
    public bool IsCancellable { get; set; }
    public bool HasReceivedReminder { get; set; }
}

public class BusinessDetailsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Image? Logo { get; set; }
    public Image? Cover { get; set; }
    public Address? Address { get; set; }
    public PhoneNumber PhoneNumber { get; set; } = default!;
}
