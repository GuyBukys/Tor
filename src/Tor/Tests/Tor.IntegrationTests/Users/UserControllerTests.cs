using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Users.Commands.AddOrUpdateDevice;
using Tor.Contracts.User;
using Tor.Domain.UserAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.Users;
public class UserControllerTests : BaseIntegrationTest
{
    public UserControllerTests(IntegrationTestWebApplicationFactory factory)
    : base(factory)
    {
    }

    [Fact]
    public async Task AddOrUpdateDevice_WhenDeviceDoesntExist_ShouldAddDevice()
    {
        Guid userId = await TestUtils.SetupUser(Context);
        var user = await Context.Users.FirstAsync();
        AddOrUpdateDeviceCommand request = Fakers.Users.AddOrUpdateDeviceCommandFaker.Generate() with
        {
            UserToken = user.UserToken,
        };

        var result = await Client.PostAsJsonAsync(UserControllerConstants.AddOrUpdateDeviceUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var userFromDb = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        userFromDb!.Devices.Should().Contain(x => x.Token == request.DeviceToken);
        var response = await result.Content.ReadFromJsonAsync<AddOrUpdateDeviceResponse>();
        response.Should().NotBeNull();
        response!.Devices.Should().BeEquivalentTo(userFromDb.Devices);
    }

    [Fact]
    public async Task CreateUser_WhenValidRequest_ShouldCreateUser()
    {
        var request = Fakers.Users.CreateUserCommandFaker.Generate();

        var result = await Client.PostAsJsonAsync(UserControllerConstants.CreateUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var userFromDb = await Context.Users.FirstOrDefaultAsync();
        userFromDb.Should().NotBeNull();
        var response = await result.Content.ReadFromJsonAsync<CreateUserResponse>();
        response.Should().NotBeNull();
        userFromDb!.Should().BeEquivalentTo(response, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetUser_WhenUserExist_ShouldReturnUser()
    {
        Guid userId = await TestUtils.SetupUser(Context);
        User user = await Context.Users.FirstAsync(x => x.Id == userId);
        string requestUri = $"{UserControllerConstants.GetUri}?userToken={user.UserToken}&appType={user.AppType}";

        var result = await Client.GetFromJsonAsync<GetUserResponse>(requestUri);

        result.Should().NotBeNull();
        user.Should().BeEquivalentTo(result!, cfg => cfg.ExcludingMissingMembers());
    }
}
