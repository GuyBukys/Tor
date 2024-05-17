using Tor.Contracts.StaffMember;
using Tor.Contracts.Tier;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.Business;

public class GetBusinessResponse
{
    public Guid Id { get; set; }
    public string InvitationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public BusinessSettings Settings { get; set; } = default!;
    public Address? Address { get; set; } = default!;
    public Image? Logo { get; set; }
    public Image? Cover { get; set; }
    public List<PhoneNumber> PhoneNumbers { get; set; } = [];
    public List<Image> Portfolio { get; set; } = [];
    public List<GetStaffMemberResponse> StaffMembers { get; set; } = [];
    public WeeklySchedule WeeklySchedule { get; set; } = default!;
    public List<SocialMedia> SocialMedias { get; set; } = [];
    public string HomepageNote { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
    public TierResponse Tier { get; set; } = default!;
}