using Mapster;
using Tor.Application.WaitingLists.Queries.GetByClient;
using Tor.Contracts.WaitingList;

namespace Tor.Api.Mappings;

public class WaitingListMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WaitingListResult, WaitingListResponse>();
        config.NewConfig<List<WaitingListResult>, GetWaitingListsResponse>()
            .Map(dest => dest.WaitingLists, src => src);
    }
}
