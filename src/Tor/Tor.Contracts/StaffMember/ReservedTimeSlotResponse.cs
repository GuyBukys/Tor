using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.StaffMember;

public class ReservedTimeSlotResponse
{
    public Guid Id { get; set; }
    public DateOnly AtDate { get; set; }
    public TimeRange TimeRange { get; set; } = default!;
    public string? Reason { get; set; } = null!;
}
