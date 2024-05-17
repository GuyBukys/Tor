using FluentAssertions;
using Tor.Application.Services.Queries.GetDefaultServices;
using Tor.Domain.CategoryAggregate.Enums;
using Tor.TestsInfra;

namespace Tor.UnitTests.Application.Services;

public class GetDefaultServicesQueryHandlerTests : UnitTestBase
{
    private readonly GetDefaultServicesQueryHandler _sut;

    public GetDefaultServicesQueryHandlerTests()
        : base()
    {
        _sut = new GetDefaultServicesQueryHandler();
    }

    [Theory]
    [InlineData(CategoryType.Barbershop)]
    [InlineData(CategoryType.Makeup)]
    [InlineData(CategoryType.NailSalon)]
    [InlineData(CategoryType.PetServices)]
    [InlineData(CategoryType.PersonalTrainer)]
    [InlineData(CategoryType.HairSalon)]
    [InlineData(CategoryType.EyebrowsAndLashes)]
    [InlineData(CategoryType.Piercing)]
    [InlineData(CategoryType.Massage)]
    [InlineData(CategoryType.PrivateTutor)]
    public async Task Handle_WhenValidCategory_ShouldReturnDefaultServices(CategoryType category)
    {
        var query = new GetDefaultServicesQuery(category);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenNoServicesForCategory_ShouldReturnEmptyList()
    {
        var query = new GetDefaultServicesQuery(0);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
