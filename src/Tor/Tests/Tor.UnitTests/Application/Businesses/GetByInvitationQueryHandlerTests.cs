using FluentAssertions;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Queries.GetByInvitation;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class GetByInvitationQueryHandlerTests : UnitTestBase
{
    private readonly Mock<IBusinessRepository> _repositoryMock;
    private readonly GetByInvitationQueryHandler _sut;

    public GetByInvitationQueryHandlerTests()
        : base()
    {
        _repositoryMock = new Mock<IBusinessRepository>();
        _sut = new GetByInvitationQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldReturnBusiness()
    {
        string invitationId = Guid.NewGuid().ToString();
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        _repositoryMock.Setup(x => x.GetByInvitation(invitationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(business);
        var query = new GetByInvitationQuery(invitationId);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(business);
        _repositoryMock.Verify(x => x.GetByInvitation(invitationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnNotFoundError()
    {
        var query = new GetByInvitationQuery(Guid.NewGuid().ToString());

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with invitation id {query.InvitationId}"));
    }
}
