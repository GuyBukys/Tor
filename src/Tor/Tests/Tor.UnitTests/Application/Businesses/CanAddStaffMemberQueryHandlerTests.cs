using FluentAssertions;
using Tor.Application.Businesses.Queries.CanAddStaffMember;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class CanAddStaffMemberQueryHandlerTests : UnitTestBase
{
    private readonly CanAddStaffMemberQueryHandler _sut;

    public CanAddStaffMemberQueryHandlerTests()
        : base()
    {
        _sut = new CanAddStaffMemberQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldNotFoundError()
    {
        var query = new CanAddStaffMemberQuery(Guid.NewGuid());

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with id {query.BusinessId}"));
    }

    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 3)]
    [InlineData(2, 3)]
    public async Task Handle_WhenStaffMemberCountIsLessThanAllowedStaffMembers_ShouldReturnTrue(int staffMemberCount, short maximumStaffMembers)
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        List<StaffMember> staffMembers = Enumerable.Range(0, staffMemberCount)
            .Select(_ => Fakers.StaffMembers.StaffMemberFaker.Generate())
            .ToList();
        business.StaffMembers.AddRange(staffMembers);
        business.Tier.FreeTrialDuration = TimeSpan.Zero;
        business.Tier.MaximumStaffMembers = maximumStaffMembers;
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var query = new CanAddStaffMemberQuery(business.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Theory]
    [InlineData(3, 3)]
    [InlineData(4, 3)]
    public async Task Handle_WhenStaffMemberCountIsGreaterOrEqualToAllowedStaffMembers_ShouldReturnFalse(int staffMemberCount, short maximumStaffMembers)
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        List<StaffMember> staffMembers = Enumerable.Range(0, staffMemberCount)
            .Select(_ => Fakers.StaffMembers.StaffMemberFaker.Generate())
            .ToList();
        business.StaffMembers.AddRange(staffMembers);
        business.Tier.FreeTrialDuration = TimeSpan.Zero;
        business.Tier.MaximumStaffMembers = maximumStaffMembers;
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var query = new CanAddStaffMemberQuery(business.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }
}
