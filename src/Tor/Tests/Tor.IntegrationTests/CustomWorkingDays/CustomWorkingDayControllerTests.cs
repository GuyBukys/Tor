using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Tor.Contracts.StaffMember;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.CustomWorkingDays;

public class CustomWorkingDayControllerTests : BaseIntegrationTest
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public CustomWorkingDayControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddOrUpdate_WhenCustomWorkingDayDoesntExist_ShouldAddCustomWorkingDayForStaffMember()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var command = Fakers.CustomWorkingDays.AddOrUpdateCustomWorkingDayCommandFaker.Generate() with { StaffMemberId = staffMember.Id };

        var result = await Client.PostAsJsonAsync(CustomWorkingDayControllerConstants.AddOrUpdateUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var scope = _factory.Services.CreateScope();
        var customWorkingDays = await Context.StaffMembers.Where(x => x.Id == command.StaffMemberId)
            .AsNoTracking()
            .Select(x => x.CustomWorkingDays)
            .FirstOrDefaultAsync();
        customWorkingDays.Should().NotBeNull();
        customWorkingDays.Should().HaveCount(1);
        customWorkingDays!.First().Should().Be(command.CustomWorkingDay);
    }

    [Fact]
    public async Task AddOrUpdate_WhenCustomWorkingDayDoesntExist_ShouldAddCustomWorkingDayForBusiness()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var command = Fakers.CustomWorkingDays.AddOrUpdateCustomWorkingDayCommandFaker.Generate() with { StaffMemberId = staffMember.Id };

        var result = await Client.PostAsJsonAsync(CustomWorkingDayControllerConstants.AddOrUpdateUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var scope = _factory.Services.CreateScope();
        var customWorkingDays = await Context.StaffMembers.Where(x => x.Id == command.StaffMemberId)
            .AsNoTracking()
            .Select(x => x.CustomWorkingDays)
            .FirstOrDefaultAsync();
        customWorkingDays.Should().NotBeNull();
        customWorkingDays.Should().HaveCount(1);
        customWorkingDays!.First().Should().Be(command.CustomWorkingDay);
    }

    [Fact]
    public async Task GetCustomWorkingDays_WhenCustomWorkingDaysExists_ShouldReturnUntilGivenDate()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        var customWorkingDays = Fakers.CustomWorkingDays.CustomWorkingDayFaker.Generate(5);
        staffMember.CustomWorkingDays.AddRange(customWorkingDays);
        await Context.SaveChangesAsync();
        string requestUri = $"{CustomWorkingDayControllerConstants.GetCustomWorkingDaysUri}?staffMemberId={staffMember.Id}&until=2030-01-01";

        var result = await Client.GetFromJsonAsync<GetCustomWorkingDaysResponse>(requestUri);

        result.Should().NotBeNull();
        result!.CustomWorkingDays.Should().NotBeEmpty();
        result!.CustomWorkingDays.Should().BeEquivalentTo(customWorkingDays);
    }

    [Fact]
    public async Task GetCustomWorkingDays_WhenNoCustomWorkingDaysExistsForStaffMember_ShouldReturnEmptyList()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        string requestUri = $"{CustomWorkingDayControllerConstants.GetCustomWorkingDaysUri}?staffMemberId={staffMember.Id}";

        var result = await Client.GetFromJsonAsync<GetCustomWorkingDaysResponse>(requestUri);

        result.Should().NotBeNull();
        result!.CustomWorkingDays.Should().BeEmpty();
    }
}
