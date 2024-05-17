using Mapster;
using Tor.Application.MessageBlasts.Common;
using Tor.Contracts.Business;

namespace Tor.Api.Mappings;

public class MessageBlastMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MessageBlastResult, MessageBlastResponse>();
        config.NewConfig<List<MessageBlastResult>, GetMessageBlastsResponse>()
            .Map(dest => dest.MessageBlasts, src => src);
    }
}
