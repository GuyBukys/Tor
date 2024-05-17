using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Services.Queries.GetServices;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;
using FluentAssertions;

namespace Tor.UnitTests.Application.Services;

public class GetServicesQueryHandlerTests : UnitTestBase
{
    private readonly GetServicesQueryHandler _sut;

    public GetServicesQueryHandlerTests()
        : base()
    {
        _sut = new GetServicesQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenStaffMemberExistsWithServices_ShouldReturnServices()
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var query = new GetServicesQuery(staffMember.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().BeEquivalentTo(staffMember.Services);
    }

    [Fact]
    public async Task Handle_WhenStaffMemberExistsWithoutServices_ShouldReturnEmptyList()
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Services.Clear();
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var query = new GetServicesQuery(staffMember.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExists_ShouldReturnNotFoundError()
    {
        var query = new GetServicesQuery(Guid.NewGuid());

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {query.StaffMemberId}"));
    }
}
