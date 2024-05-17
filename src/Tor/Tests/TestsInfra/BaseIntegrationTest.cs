using Tor.Infrastructure.Authentication;
using Tor.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tor.TestsInfra;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebApplicationFactory>, IDisposable
{
    protected readonly HttpClient Client;
    internal readonly TorDbContext Context;

    public BaseIntegrationTest(IntegrationTestWebApplicationFactory factory)
    {
        Client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<TorDbContext>();
        Context.Database.EnsureCreated();

        FirebaseSettings firebaseSettings = factory.Services.GetRequiredService<IOptions<FirebaseSettings>>().Value;
        Client.DefaultRequestHeaders.Add(FirebaseConstants.FirebaseAppCheckHeader, firebaseSettings.DebugToken);
    }

    public void Dispose()
    {
        Context?.Database.EnsureDeleted();
        Context?.Dispose();
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}
