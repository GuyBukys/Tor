using Domain;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Domain.BusinessAggregate.Entities;

public sealed class Service : Entity<Guid>
{
    public Service(Guid id) : base(id)
    {
        Id = id;
    }

    public Guid StaffMemberId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LocationType Location { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public AmountDetails Amount { get; set; } = default!;
    public List<Duration> Durations { get; set; } = [];

    public StaffMember StaffMember { get; set; } = default!;

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public Service()
        : base(Guid.Empty)
    {

    }
    #endregion
}
