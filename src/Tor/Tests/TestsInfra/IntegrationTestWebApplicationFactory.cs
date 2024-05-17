using Medallion.Threading;
using Medallion.Threading.Postgres;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.PostgreSql;
using Tor.Application.Abstractions;
using Tor.Application.Common.Behaviors;
using Tor.Domain.CategoryAggregate.Enums;
using Tor.Domain.Common.Enums;
using Tor.Domain.Common.ValueObjects;
using Tor.Infrastructure.Persistence;
using Xunit;

namespace Tor.TestsInfra;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithName($"db-integration-tests-{Guid.NewGuid().ToString()[^5..]}")
        .WithDatabase("Tordb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public readonly Mock<IStorageManager> StorageManagerMock = new();
    public readonly Mock<IPushNotificationSender> PushNotificationSenderMock = new();
    public readonly Mock<IIsraelDateTimeProvider> IsraelDateTimeProviderMock = new();

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("IsTestMode", "true");

        builder.ConfigureTestServices(services =>
        {
            var signUrlDescriptor = services.SingleOrDefault(s => s.ImplementationType == typeof(SignUrlsBehavior<,>));
            if (signUrlDescriptor is not null)
            {
                services.Remove(signUrlDescriptor);
            }

            var dbContextOptionsDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<TorDbContext>));
            if (dbContextOptionsDescriptor is not null)
            {
                services.Remove(dbContextOptionsDescriptor);
            }

            string connectionString = $"{_dbContainer.GetConnectionString()};Include Error Detail=true";
            services.AddDbContext<TorDbContext>(options =>
                options.UseNpgsql(connectionString));

            var distributedLockProviderDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IDistributedLockProvider));
            if (distributedLockProviderDescriptor is not null)
            {
                services.Remove(distributedLockProviderDescriptor);
            }

            services.AddSingleton<IDistributedLockProvider>(_ => new PostgresDistributedSynchronizationProvider(connectionString));

            services.AddSingleton(so => StorageManagerMock.Object);
            services.AddSingleton(so => PushNotificationSenderMock.Object);
            services.AddSingleton(so => IsraelDateTimeProviderMock.Object);

            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Clear();
            });
        });

        SetupMocks();

        base.ConfigureWebHost(builder);
    }

    private void SetupMocks()
    {
        StorageManagerMock.Setup(x => x.GetDefaultImage(It.IsAny<ImageType>(), It.IsAny<EntityType>(), It.IsAny<CategoryType>()))
            .Returns(new Image("image name", new Uri("https://google.com/")));
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}
