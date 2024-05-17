using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Clients.Commands.UpdateFavoriteBusiness;
using Tor.Contracts.Client;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;
using Tor.Domain.ClientAggregate.Entities;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.Clients;

public class ClientControllerTests : BaseIntegrationTest
{
    private readonly Mock<IPushNotificationSender> _pushNotificationSenderMock;
    private readonly Mock<IStorageManager> _storageManagerMock;

    public ClientControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
        _storageManagerMock = factory.StorageManagerMock;
        _pushNotificationSenderMock = factory.PushNotificationSenderMock;
    }

    [Fact]
    public async Task GetAppointments_WhenAppointmentsExists_ShouldReturnAppointments()
    {
        Client client = await TestUtils.SetupClient(Context);
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        List<Appointment> appointments = await TestUtils.SetupAppointments(staffMember, Context, client.Id);
        string requestUri = $"{ClientControllerConstants.GetAppointmentsUri}?clientId={client.Id}";

        var result = await Client.GetFromJsonAsync<GetClientAppointmentsResponse>(requestUri);

        result.Should().NotBeNull();
        appointments.Should().BeEquivalentTo(result!.Appointments, cfg => cfg.ExcludingMissingMembers().Excluding(x => x.ScheduledFor));
        appointments.Select(x => x.StaffMember.Business).Should().BeEquivalentTo(result!.Appointments.Select(x => x.BusinessDetails), cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetById_WhenValidRequest_ShouldGetClient()
    {
        var client = await TestUtils.SetupClient(Context);
        string requestUri = $"{ClientControllerConstants.GetByIdUri}?id={client.Id}";

        var result = await Client.GetFromJsonAsync<ClientResponse>(requestUri);

        result.Should().NotBeNull();
        client.Should().BeEquivalentTo(result, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Deactivate_WhenValidRequest_ShouldDeactivateClient()
    {
        var client = await TestUtils.SetupClient(Context);
        var user = Fakers.Users.UserFaker.Generate();
        user.EntityId = client.Id;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        string requestUri = $"{ClientControllerConstants.DeactivateUri}?clientId={client.Id}";

        var result = await Client.PostAsync(requestUri, null);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(client).ReloadAsync();
        client.IsActive.Should().BeFalse();
        bool isUserExists = await Context.Users.AnyAsync(x => x.EntityId == client.Id);
        isUserExists.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WhenValidRequest_ShouldCreateClient()
    {
        User user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.ClientApp;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var request = Fakers.Clients.CreateClientCommandFaker.Generate() with
        {
            UserId = user.Id,
        };
        _storageManagerMock.Setup(x => x.IsFileExists(request.ProfileImage!.Name, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await Client.PostAsJsonAsync(ClientControllerConstants.CreateUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var clientFromDb = await Context.Clients.FirstOrDefaultAsync();
        clientFromDb.Should().NotBeNull();
        clientFromDb.Should().BeEquivalentTo(request, cfg => cfg.ExcludingMissingMembers());
        var response = JsonConvert.DeserializeObject<CreateClientResponse>(
            await result.Content.ReadAsStringAsync());
        response.Should().NotBeNull();
        response!.ClientId.Should().Be(clientFromDb!.Id);
        await Context.Entry(user).ReloadAsync();
        user.EntityId.Should().Be(clientFromDb!.Id);
        _pushNotificationSenderMock.Verify(x => x.AddToGroup(It.Is<AddToGroupRequest>(x => x.GroupName == Tor.Application.Constants.Groups.ClientsNotificationGroup)), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateFavoriteBusiness_WhenIsFavoriteTrue_ShouldAddOrUpdateFavoriteBusiness(bool isExists)
    {
        var client = await TestUtils.SetupClient(Context);
        var business = await TestUtils.SetupBusiness(Context);
        if (isExists)
        {
            await Context.FavoriteBusinesses.AddAsync(new FavoriteBusiness(Guid.NewGuid())
            {
                BusinessId = business.Id,
                ClientId = client.Id,
            });
            await Context.SaveChangesAsync();
        }
        var command = new UpdateFavoriteBusinessCommand(client.Id, business.Id, true, true);

        var result = await Client.PostAsJsonAsync(ClientControllerConstants.UpdateFavoriteBusinessUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var favoriteBusiness = await Context.FavoriteBusinesses.SingleOrDefaultAsync(x =>
            x.BusinessId == business.Id && x.ClientId == client.Id);
        favoriteBusiness.Should().NotBeNull();
        await Context.Entry(favoriteBusiness!).ReloadAsync();
        favoriteBusiness!.MuteNotifications.Should().Be(command.MuteNotifications);
    }

    [Fact]
    public async Task UpdateFavoriteBusiness_WhenIsFavoriteFalse_ShouldDeleteFavoriteBusiness()
    {
        var client = await TestUtils.SetupClient(Context);
        var business = await TestUtils.SetupBusiness(Context);
        await Context.FavoriteBusinesses.AddAsync(new FavoriteBusiness(Guid.NewGuid())
        {
            BusinessId = business.Id,
            ClientId = client.Id,
        });
        await Context.SaveChangesAsync();
        var command = new UpdateFavoriteBusinessCommand(client.Id, business.Id, false, false);

        var result = await Client.PostAsJsonAsync(ClientControllerConstants.UpdateFavoriteBusinessUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var favoriteBusiness = await Context.FavoriteBusinesses.SingleOrDefaultAsync(x =>
            x.BusinessId == business.Id && x.ClientId == client.Id);
        favoriteBusiness.Should().BeNull();
    }
}
