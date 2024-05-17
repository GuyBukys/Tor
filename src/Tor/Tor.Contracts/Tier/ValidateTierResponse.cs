namespace Tor.Contracts.Tier;

public class ValidateTierResponse
{
    public bool IsValid { get; set; }
    public bool OpenPaywall { get; set; }
    public TierResponse? RequiredTier { get; set; } = null!;
}
