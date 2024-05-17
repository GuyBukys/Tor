using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tor.Infrastructure.Persistence;
using Xunit;

namespace Tor.TestsInfra;

public abstract class UnitTestBase : IAsyncLifetime
{
    internal IMapper Mapper { get; set; }
    internal TorDbContext Context { get; set; }

    protected UnitTestBase()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(
            typeof(Application.DependencyInjection).Assembly,
            typeof(Api.DependencyInjection).Assembly);

        Mapper = new Mapper(config);

        var options = new DbContextOptionsBuilder<TorDbContext>()
            .UseInMemoryDatabase($"UnitTests:{Guid.NewGuid()}")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        Context = new TorDbContext(options);
    }

    public Task InitializeAsync()
    {
        return Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Context!.Database.EnsureDeletedAsync();
        await Context!.DisposeAsync();
    }
}
