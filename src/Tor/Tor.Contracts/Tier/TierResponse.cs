using Tor.Domain.TierAggregate.Enums;

namespace Tor.Contracts.Tier;

public class TierResponse
{
    public TierType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool AppointmentApprovals { get; set; }
    public bool AppointmentReminders { get; set; }
    public bool MessageBlasts { get; set; }
    public bool Payments { get; set; }
    public short MaximumStaffMembers { get; set; }
    public string ExternalReference { get; set; } = string.Empty;
    public TimeSpan FreeTrialDuration { get; set; }
}
