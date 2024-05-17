using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Abstractions;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.ReservedTimeSlots;

public class ReservedTimeSlotControllerTests : BaseIntegrationTest
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public ReservedTimeSlotControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Update_WhenValidRequest_ShouldUpdateReservedTimeSlot()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var reservedTimeSlot = Fakers.ReservedTimeSlots.ReservedTimeSlotFaker.Generate();
        reservedTimeSlot.StaffMemberId = staffMember.Id;
        await Context.ReservedTimeSlots.AddAsync(reservedTimeSlot);
        await Context.SaveChangesAsync();
        var command = Fakers.ReservedTimeSlots.UpdateReservedTimeSlotCommandFaker.Generate() with { Id = reservedTimeSlot.Id };

        var result = await Client.PutAsJsonAsync(ReservedTimeSlotControllerConstants.UpdateUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITorDbContext>();
        var reservedTimeSlotFromDb = await context.ReservedTimeSlots.FirstAsync(x => x.Id == reservedTimeSlot.Id);
        reservedTimeSlotFromDb.Should().BeEquivalentTo(command, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Add_WhenValidRequest_ShouldAddReservedTimeSlot()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var command = Fakers.ReservedTimeSlots.AddReservedTimeSlotCommandFaker.Generate() with { StaffMemberId = staffMember.Id };

        var result = await Client.PostAsJsonAsync(ReservedTimeSlotControllerConstants.AddUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var reservedTimeSlotFromDb = await Context.ReservedTimeSlots.FirstOrDefaultAsync(x => x.StaffMemberId == staffMember.Id);
        reservedTimeSlotFromDb.Should().NotBeNull();
        reservedTimeSlotFromDb.Should().BeEquivalentTo(command, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Delete_WhenValidRequest_ShouldAddReservedTimeSlot()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var reservedTimeSlot = Fakers.ReservedTimeSlots.ReservedTimeSlotFaker.Generate();
        reservedTimeSlot.StaffMemberId = staffMember.Id;
        await Context.ReservedTimeSlots.AddAsync(reservedTimeSlot);
        await Context.SaveChangesAsync();
        string requestUri = $"{ReservedTimeSlotControllerConstants.DeleteUri}?id={reservedTimeSlot.Id}";

        var result = await Client.DeleteAsync(requestUri);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var reservedTimeSlotFromDb = await Context.ReservedTimeSlots.FirstOrDefaultAsync(x => x.Id == reservedTimeSlot.Id);
        reservedTimeSlotFromDb.Should().BeNull();
    }
}
