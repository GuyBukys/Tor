using Domain;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.CategoryAggregate;
using Tor.Domain.ClientAggregate;
using Tor.Domain.Common.ValueObjects;
using Tor.Domain.MessageBlastAggregate;
using Tor.Domain.TierAggregate;

namespace Tor.Domain.BusinessAggregate;

public sealed class Business : AggregateRoot<Guid>
{
    public Business(Guid id) : base(id) { }

    public Guid? TierId { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InvitationId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public Address Address { get; set; } = default!;
    public List<PhoneNumber> PhoneNumbers { get; set; } = [];
    public List<SocialMedia> SocialMedias { get; set; } = [];
    public List<Location> Locations { get; set; } = [];
    public BusinessSettings Settings { get; set; } = default!;
    public string HomepageNote { get; set; } = string.Empty;
    public Image? Logo { get; set; } = null!;
    public Image? Cover { get; set; } = null!;
    public List<Image> Portfolio { get; set; } = [];
    public string? NotesForAppointment { get; set; } = null!;
    public string ReferralCode { get; set; } = string.Empty;
    public Guid? ReferringBusinessId { get; set; } = null!;

    public Tier? Tier { get; set; } = null!;
    public List<StaffMember> StaffMembers { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<Client> Clients { get; set; } = [];
    public List<Client> BlockedClients { get; set; } = [];
    public List<MessageBlast> MessageBlasts { get; set; } = [];
    public Business? ReferringBusiness { get; set; } = null!;

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public Business()
        : base(Guid.Empty)
    {

    }
    #endregion
}
