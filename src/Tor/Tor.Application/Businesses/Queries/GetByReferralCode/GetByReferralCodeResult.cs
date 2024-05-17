namespace Tor.Application.Businesses.Queries.GetByReferralCode;

public record GetByReferralCodeResult(
    Guid BusinessId,
    string BusinessName);
