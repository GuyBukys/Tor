using FluentValidation;
using Mapster;
using MapsterMapper;
using Medallion.Threading;
using Medallion.Threading.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tor.Application.Appointments.Common;
using Tor.Application.Common.Behaviors;
using Tor.Application.Common.Settings;

namespace Tor.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(DependencyInjection).Assembly);
        services.AddSingleton<IMapper>(new Mapper(config));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        AddServices(services, configuration);

        AddMediatR(services);

        services.Configure<TwoKingsSettings>(configuration.GetSection(nameof(TwoKingsSettings)));

        return services;
    }

    private static void AddMediatR(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(UtcConverterBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(SignUrlsBehavior<,>));
        });
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAvailableTimesCalculator, AvailableTimesCalculator>();

        string connectionString = configuration.GetConnectionString("TorConnectionString")!;
        services.AddSingleton<IDistributedLockProvider>(_ => new PostgresDistributedSynchronizationProvider(connectionString));
    }
}
