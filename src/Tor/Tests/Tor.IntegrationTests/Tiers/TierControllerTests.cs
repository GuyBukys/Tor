using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Tor.Application;
using Tor.Application.Tiers.Commands.UpdateTier;
using Tor.Contracts.Tier;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.TierAggregate;
using Tor.Domain.TierAggregate.Enums;
using Tor.TestsInfra;

namespace Tor.IntegrationTests.Tiers;

public class TierControllerTests : BaseIntegrationTest
{
    public TierControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ValidateTier_WhenValidExternalReference_ShouldReturnIsValidTrue()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Business business = await Context.Businesses.Include(x => x.StaffMembers).FirstAsync();
        string requestUri = $"{TierControllerConstants.ValidateTierUri}?staffMemberId={staffMember.Id}&externalReference={business.Tier.ExternalReference}";

        var result = await Client.GetFromJsonAsync<ValidateTierResponse>(requestUri);

        result.Should().NotBeNull();
        result!.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateTier_WhenValidCommand_ShouldUpdateSuccessfully()
    {
        Business business = await TestUtils.SetupBusiness(Context);
        Tier premiumTier = await Context.Tiers.FirstAsync(x => x.Type == TierType.Premium);
        var command = new UpdateTierCommand(business.Id, premiumTier.Type);

        var result = await Client.PutAsJsonAsync(TierControllerConstants.UpdateTierUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(business).ReloadAsync();

        Tier tierFromDb = await Context.Tiers.FirstAsync(x => x.Id == business.TierId);
        premiumTier.Should().BeEquivalentTo(tierFromDb, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task UpdateTier_WhenNullTierAndNotFreeTrial_ShouldRemoveTierSuccessfully()
    {
        Business business = await TestUtils.SetupBusiness(Context);
        business.CreatedDateTime = DateTime.UtcNow.Add(-(Constants.FreeTrialDuration + TimeSpan.FromDays(1)));
        await Context.SaveChangesAsync();
        var command = new UpdateTierCommand(business.Id, null);

        var result = await Client.PutAsJsonAsync(TierControllerConstants.UpdateTierUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(business).ReloadAsync();
        business.TierId.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTier_WhenNullTierAndFreeTrial_ShouldRemoveTierSuccessfully()
    {
        Business business = await TestUtils.SetupBusiness(Context);
        business.TierId = await Context.Tiers.Where(x => x.Type == TierType.Premium).Select(x => x.Id).FirstAsync();
        await Context.SaveChangesAsync();
        var command = new UpdateTierCommand(business.Id, null);

        var result = await Client.PutAsJsonAsync(TierControllerConstants.UpdateTierUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(business).ReloadAsync();
        business.TierId.Should().NotBeNull();
    }
}
