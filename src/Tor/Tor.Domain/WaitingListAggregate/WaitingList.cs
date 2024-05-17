using Domain;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;

namespace Tor.Domain.WaitingListAggregate;

public sealed class WaitingList : Entity<Guid>
{
    public WaitingList(Guid id) : base(id)
    {
    }

    public Guid StaffMemberId { get; set; }
    public DateOnly AtDate { get; set; }

    public StaffMember StaffMember { get; set; } = default!;
    public List<Client> Clients { get; set; } = [];

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public WaitingList()
        : base(Guid.Empty)
    {

    }
    #endregion
}
