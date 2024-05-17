using FluentAssertions;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Queries.GetByStaffMember;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class GetByStaffMemberQueryHandlerTests : UnitTestBase
{
    private readonly Mock<IBusinessRepository> _repositoryMock;
    private readonly GetByStaffMemberQueryHandler _sut;

    public GetByStaffMemberQueryHandlerTests()
        : base()
    {
        _repositoryMock = new Mock<IBusinessRepository>();
        _sut = new GetByStaffMemberQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldReturnBusiness()
    {
        Guid staffMemberId = Guid.NewGuid();
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        _repositoryMock.Setup(x => x.GetByStaffMember(staffMemberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(business);
        var query = new GetByStaffMemberQuery(staffMemberId);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(business);
        _repositoryMock.Verify(x => x.GetByStaffMember(staffMemberId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnNotFoundError()
    {
        var query = new GetByStaffMemberQuery(Guid.NewGuid());

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with staff member id {query.StaffMemberId}"));
    }
}
