using Mapster;
using Tor.Application.Tiers.Queries.ValidateTier;
using Tor.Contracts.Tier;

namespace Tor.Api.Mappings;

public class TierMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ValidateTierResult, ValidateTierResponse>();
    }
}
