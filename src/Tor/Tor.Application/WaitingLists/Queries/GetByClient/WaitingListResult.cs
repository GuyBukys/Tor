using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.WaitingLists.Queries.GetByClient;

public record WaitingListResult(
    DateOnly AtDate,
    Guid StaffMemberId,
    Guid BusinessId,
    string BusinessName,
    Image BusinessLogo);
