using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Tiers.Queries.ValidateTier;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.TierAggregate.Enums;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Tiers;

public class ValidateTierQueryHandlerTests : UnitTestBase
{
    private readonly ValidateTierQueryHandler _sut;

    public ValidateTierQueryHandlerTests()
        : base()
    {
        _sut = new ValidateTierQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenFreeTrial_ShouldReturnValid()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        business.StaffMembers.Add(staffMember);
        business.Tier.FreeTrialDuration = TimeSpan.FromDays(100);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var query = new ValidateTierQuery(staffMember.Id, business.Tier.ExternalReference);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenValidTier_ShouldReturnValid()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        business.Tier.MaximumStaffMembers = 5;
        var staffMembers = Fakers.StaffMembers.StaffMemberFaker.Generate(5);
        business.StaffMembers.AddRange(staffMembers);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var query = new ValidateTierQuery(staffMembers.First().Id, business.Tier.ExternalReference);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsValid.Should().BeTrue();
        result.Value.RequiredTier.Should().BeNull();
    }

    [Theory]
    [InlineData(PositionType.BusinessOwner)]
    [InlineData(PositionType.RegularStaffMember)]
    public async Task Handle_WhenStaffMemberCountGreaterThanTier_ShouldReturnIsValidFalse(PositionType position)
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        business.Tier.FreeTrialDuration = TimeSpan.Zero;
        business.Tier.MaximumStaffMembers = 5;
        var staffMembers = Fakers.StaffMembers.StaffMemberFaker.Generate(6);
        staffMembers[0].Position = position;
        business.StaffMembers.AddRange(staffMembers);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var query = new ValidateTierQuery(staffMembers[0].Id, business.Tier.ExternalReference);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsValid.Should().BeFalse();
        result.Value.RequiredTier.Should().NotBeNull();
        result.Value.RequiredTier!.Type.Should().Be(TierType.Enterprise);
        result.Value.OpenPaywall.Should().Be(position == PositionType.BusinessOwner);
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnFailedResult()
    {
        var query = new ValidateTierQuery(Guid.NewGuid(), "external reference");

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business for staff member with id {query.StaffMemberId}"));
    }
}
