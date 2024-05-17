using FluentAssertions;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Businesses.Queries.GetAllBusinesses;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Common.Models;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.ClientAggregate;
using Tor.Domain.ClientAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class GetAllBusinessesQueryHandlerTests : UnitTestBase
{
    private readonly Mock<IBusinessRepository> _repositoryMock;
    private readonly GetAllBusinessesQueryHandler _sut;

    public GetAllBusinessesQueryHandlerTests()
        : base()
    {
        _repositoryMock = new Mock<IBusinessRepository>();
        _sut = new GetAllBusinessesQueryHandler(Context, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldReturnBusinesses()
    {
        var now = new DateTime(2024, 1, 1, 10, 0, 0);
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(5);
        SetupRepository(businesses);
        Client client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        await Context.FavoriteBusinesses.AddRangeAsync(businesses.Select(x => new FavoriteBusiness(Guid.NewGuid())
        {
            BusinessId = x.Id,
            ClientId = client.Id,
        }));
        await Context.SaveChangesAsync();
        var query = new GetAllBusinessesQuery(client.Id, 1, 10, null, null, null);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.GetAll(It.IsAny<GetAllBusinessesInput>(), It.IsAny<CancellationToken>()), Times.Once);
        businesses.Should().BeEquivalentTo(result.Value.Items, cfg => cfg.ExcludingMissingMembers());
        result.Value.Items.Should().AllSatisfy(x =>
        {
            x.IsFavorite.Should().BeTrue();
        });
    }

    [Fact]
    public async Task Handle_WhenValidQueryAndIsNotFavorite_ShouldReturnIsFavoriteFalse()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        SetupRepository([business]);
        Client client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        var query = new GetAllBusinessesQuery(client.Id, 1, 10, null, null, null);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.GetAll(It.IsAny<GetAllBusinessesInput>(), It.IsAny<CancellationToken>()), Times.Once);
        business.Should().BeEquivalentTo(result.Value.Items.First(), cfg => cfg.ExcludingMissingMembers());
        result.Value.Items.First().IsFavorite.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenNoBusinesses_ShouldReturnEmptyList()
    {
        _repositoryMock.Setup(x => x.GetAll(It.IsAny<GetAllBusinessesInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedList<BusinessOutput>([], 1, 10, 100));
        Client client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        var query = new GetAllBusinessesQuery(client.Id, 1, 10, null, null, null);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.GetAll(It.IsAny<GetAllBusinessesInput>(), It.IsAny<CancellationToken>()), Times.Once);
        result.Value.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenClientDoesntExist_ShouldReturnNotFoundError()
    {
        var query = new GetAllBusinessesQuery(Guid.NewGuid(), 1, 10, null, null, null);

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find client with id {query.ClientId}"));
    }

    private void SetupRepository(List<Business> businesses)
    {
        var businessOutputs = businesses.ConvertAll(x => new BusinessOutput(
            x.Id,
            x.Name,
            x.Description,
            x.Logo!,
            x.Cover!,
            x.Address,
            x.PhoneNumbers.First(),
            null));

        _repositoryMock.Setup(x => x.GetAll(It.IsAny<GetAllBusinessesInput>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            new PagedList<BusinessOutput>(businessOutputs, 1, 100, 1000));
    }
}
