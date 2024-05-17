namespace Tor.Contracts.Business;

public class GetByReferralCodeResponse
{
    public Guid BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
}
