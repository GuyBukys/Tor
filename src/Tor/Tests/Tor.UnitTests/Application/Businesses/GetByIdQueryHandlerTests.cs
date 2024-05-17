using FluentAssertions;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Queries.GetById;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class GetByIdQueryHandlerTests : UnitTestBase
{
    private readonly Mock<IBusinessRepository> _repositoryMock;
    private readonly GetByIdQueryHandler _sut;

    public GetByIdQueryHandlerTests()
        : base()
    {
        _repositoryMock = new Mock<IBusinessRepository>();
        _sut = new GetByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldReturnBusiness()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        _repositoryMock.Setup(x => x.GetById(business.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(business);
        var query = new GetByIdQuery(business.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(business);
        _repositoryMock.Verify(x => x.GetById(business.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnNotFoundError()
    {
        var query = new GetByIdQuery(Guid.NewGuid());

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with id {query.Id}"));
    }
}
