using Domain;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Domain.BusinessAggregate.Entities;

public sealed class ReservedTimeSlot : Entity<Guid>
{
    public ReservedTimeSlot(Guid id) : base(id)
    {
        Id = id;
    }

    public Guid StaffMemberId { get; set; }
    public DateOnly AtDate { get; set; }
    public TimeRange TimeRange { get; set; } = default!;
    public string? Reason { get; set; } = null!;
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }

    public StaffMember StaffMember { get; set; } = default!;

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public ReservedTimeSlot()
        : base(Guid.Empty)
    {

    }
    #endregion
}
