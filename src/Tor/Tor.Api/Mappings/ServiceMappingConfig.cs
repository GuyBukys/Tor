using Mapster;
using Tor.Application.Services.Queries.GetDefaultServices;
using Tor.Contracts.Service;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Api.Mappings;

public class ServiceMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Service, ServiceResponse>();
        config.NewConfig<List<Service>, GetServicesResponse>()
            .Map(dest => dest.Services, src => src);

        config.NewConfig<DefaultService, DefaultServiceResponse>();
        config.NewConfig<List<DefaultService>, GetDefaultServicesResponse>()
            .Map(dest => dest.DefaultServices, src => src);
    }
}
