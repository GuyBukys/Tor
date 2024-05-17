using CrystalQuartz.AspNetCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Simpl;
using Tor.Application.Abstractions;
using Tor.Infrastructure.Authentication;
using Tor.Infrastructure.Jobs.AppointmentReminder;
using Tor.Infrastructure.Jobs.ScheduleAppointmentReminder;
using Tor.Infrastructure.Maps;
using Tor.Infrastructure.Notifications;
using Tor.Infrastructure.Persistence;
using Tor.Infrastructure.Persistence.Repositories;
using Tor.Infrastructure.Providers;
using Tor.Infrastructure.Storage;

namespace Tor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("TorConnectionString")!;

        services.AddDbContext<TorDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        }, ServiceLifetime.Transient);
        services.AddTransient<ITorDbContext, TorDbContext>();

        AddRepositories(services);
        AddProviders(services);
        AddGoogleServices(services, configuration);

        if (configuration.GetValue("Quartz:IsEnabled", false))
        {
            AddQuartz(services, connectionString);
        }

        return services;
    }

    public static async Task<WebApplication> UseInfrastructure(this WebApplication app, IConfiguration configuration)
    {
        using var scope = app.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TorDbContext>();
        await dbContext.Database.MigrateAsync();

        app.UseMiddleware<FirebaseMiddleware>();

        if (configuration.GetValue("Quartz:IsEnabled", false))
        {
            IScheduler scheduler = await app.Services.GetRequiredService<ISchedulerFactory>().GetScheduler();
            app.UseCrystalQuartz(() => scheduler);
        }

        return app;
    }

    private static void AddGoogleServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(FirebaseConstants.GoogleApiClient, client => client.BaseAddress = new Uri("https://firebaseappcheck.googleapis.com/"));
        services.Configure<FirebaseSettings>(configuration.GetSection(nameof(FirebaseSettings)));
        services.AddScoped<FirebaseMiddleware>();

        services.AddHttpClient(MapsConstants.MapsApiClient, client => client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/"));
        services.Configure<MapsSettings>(configuration.GetSection(nameof(MapsSettings)));
        services.AddScoped<IMapsService, MapsService>();

        if (!configuration.GetValue("IsTestMode", false))
        {
            GoogleCredential googleCredential = ConfigureGoogleCredentials(services, configuration);

            services.Configure<GoogleStorageSettings>(configuration.GetSection(nameof(GoogleStorageSettings)));
            services.AddScoped<IStorageManager, StorageManager>();

            services.Configure<FcmSettings>(configuration.GetSection(nameof(FcmSettings)));
            FirebaseApp.Create(new AppOptions() { Credential = googleCredential });
            services.AddScoped<IPushNotificationSender, PushNotificationSender>();
        }
    }

    private static GoogleCredential ConfigureGoogleCredentials(IServiceCollection services, IConfiguration configuration)
    {
        GoogleStorageSettings settings = configuration
            .GetSection(nameof(GoogleStorageSettings))
            .Get<GoogleStorageSettings>()!;

        GoogleCredential googleCredential = GoogleCredential.FromJson(settings.AuthFilePath);

        services.AddScoped(_ => googleCredential);

        return googleCredential;
    }

    private static void AddQuartz(IServiceCollection services, string connectionString)
    {
        services.AddQuartz(cfg => cfg
            .UsePersistentStore(store =>
            {
                store.UsePostgres(connectionString);
                store.UseSerializer<JsonObjectSerializer>();
            }));

        services.AddQuartzHostedService(options =>
        {
            options.AwaitApplicationStarted = true;
            options.WaitForJobsToComplete = true;
        });

        services.ConfigureOptions<ScheduleAppointmentReminderJobSetup>();
        services.ConfigureOptions<AppointmentReminderJobSetup>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IBusinessRepository, BusinessRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
    }

    private static void AddProviders(IServiceCollection services)
    {
        services.AddSingleton<IIsraelDateTimeProvider, IsraelDateTimeProvider>();
    }
}
