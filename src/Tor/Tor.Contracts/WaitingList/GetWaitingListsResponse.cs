using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.WaitingList;
public class GetWaitingListsResponse
{
    public List<WaitingListResponse> WaitingLists { get; set; } = [];
}

public class WaitingListResponse
{
    public DateOnly AtDate { get; set; }
    public Guid StaffMemberId { get; set; }
    public Guid BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public Image BusinessLogo { get; set; } = default!;
}
