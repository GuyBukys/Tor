using Mapster;
using Tor.Contracts.User;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Api.Mappings;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, CreateUserResponse>();

        config.NewConfig<User, GetUserResponse>();

        config.NewConfig<List<Device>, AddOrUpdateDeviceResponse>()
            .Map(dest => dest.Devices, src => src);
    }
}
