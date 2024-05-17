using FluentAssertions;
using Tor.Application.Businesses.Queries.IsBusinessExists;
using Tor.Domain.BusinessAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class IsBusinessExistsQueryHandlerTests : UnitTestBase
{
    private readonly IsBusinessExistsQueryHandler _sut;

    public IsBusinessExistsQueryHandlerTests()
        : base()
    {
        _sut = new IsBusinessExistsQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldReturnTrue()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var query = new IsBusinessExistsQuery(business.Email);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenBusinessWithEmailExistsButNotActive_ShouldReturnFalse()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        business.IsActive = false;
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var query = new IsBusinessExistsQuery(business.Email);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnFalse()
    {
        var query = new IsBusinessExistsQuery("email that doesnt exist");

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }
}
