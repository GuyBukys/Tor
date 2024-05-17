using Tor.Domain.TierAggregate;

namespace Tor.Application.Tiers.Queries.ValidateTier;

public record ValidateTierResult(
    bool IsValid,
    bool OpenPaywall,
    Tier? RequiredTier);
