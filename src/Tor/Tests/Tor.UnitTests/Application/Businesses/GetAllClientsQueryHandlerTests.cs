using FluentAssertions;
using Tor.Application.Businesses.Queries.GetAllClients;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.ClientAggregate;
using Tor.Domain.ClientAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class GetAllClientsQueryHandlerTests : UnitTestBase
{
    private readonly GetAllClientsQueryHandler _sut;

    public GetAllClientsQueryHandlerTests()
        : base()
    {
        _sut = new GetAllClientsQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenClientsExist_ShouldGetAllClients()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        List<Client> clientsWhoBookedAppointments = Fakers.Clients.ClientFaker.Generate(2);
        business.Clients.AddRange(clientsWhoBookedAppointments);
        await Context.SaveChangesAsync();
        List<Client> clientsWhoMarkedAsFavorites = Fakers.Clients.ClientFaker.Generate(2);
        await Context.Clients.AddRangeAsync(clientsWhoMarkedAsFavorites);
        await Context.SaveChangesAsync();
        List<Client> blockedClients = Fakers.Clients.ClientFaker.Generate(2);
        await Context.Clients.AddRangeAsync(blockedClients);
        await Context.SaveChangesAsync();
        business.BlockedClients.AddRange(blockedClients);
        var favoriteClients = clientsWhoMarkedAsFavorites.Select(x => new FavoriteBusiness(Guid.NewGuid())
        {
            ClientId = x.Id,
            BusinessId = business.Id,
        });
        await Context.FavoriteBusinesses.AddRangeAsync(favoriteClients);
        await Context.SaveChangesAsync();
        var query = new GetAllClientsQuery(business.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        clientsWhoMarkedAsFavorites.Should().BeEquivalentTo(result.Value.ClientsWhoMarkedAsFavorite, cfg => cfg.ExcludingMissingMembers());
        clientsWhoBookedAppointments.Should().BeEquivalentTo(result.Value.ClientsWhoBookedAnAppointment, cfg => cfg.ExcludingMissingMembers());
        blockedClients.Should().BeEquivalentTo(result.Value.BlockedClients, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnNotFoundError()
    {
        var query = new GetAllClientsQuery(Guid.NewGuid());

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with id {query.Id}"));
    }
}
