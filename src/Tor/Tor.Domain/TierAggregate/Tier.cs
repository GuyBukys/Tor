using Domain;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.TierAggregate.Enums;

namespace Tor.Domain.TierAggregate;

public sealed class Tier : Entity<Guid>
{
    public Tier(Guid id) : base(id)
    {
    }

    public TierType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool AppointmentApprovals { get; set; }
    public bool AppointmentReminders { get; set; }
    public bool MessageBlasts { get; set; }
    public bool Payments { get; set; }
    public int MaximumStaffMembers { get; set; }
    public string ExternalReference { get; set; } = string.Empty;
    public TimeSpan FreeTrialDuration { get; set; }

    public List<Business> Businesses { get; set; } = [];

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public Tier()
        : base(Guid.Empty)
    {

    }
    #endregion
}
