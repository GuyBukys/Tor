using Mapster;
using MapsterMapper;

namespace Tor.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(DependencyInjection).Assembly);
        services.AddSingleton<IMapper>(new Mapper(config));

        return services;
    }
}
