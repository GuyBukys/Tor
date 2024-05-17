using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Abstractions;
using Tor.Application.StaffMembers.Commands.AddStaffMember;
using Tor.Application.StaffMembers.Commands.UpdateStaffMemberSettings;
using Tor.Contracts.StaffMember;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.UserAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.StaffMembers;

public class StaffMemberControllerTests : BaseIntegrationTest
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public StaffMemberControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetSchedule_WhenAppointmentsAndRtsExists_ShouldReturnAppointmentsAndRtsUntilGivenDate()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        List<Appointment> appointments = await TestUtils.SetupAppointments(staffMember, Context);
        List<ReservedTimeSlot> reservedTimeSlots = await TestUtils.SetupReservedTimeSlots(staffMember, Context);
        string requestUri = $"{StaffMemberControllerConstants.GetScheduleUri}?staffMemberId={staffMember.Id}&until=2030-01-01";

        var result = await Client.GetFromJsonAsync<GetScheduleResponse>(requestUri);

        result.Should().NotBeNull();
        var orderedAppointmentDates = appointments.Select(x => x.ScheduledFor).Order().ToList();
        var orderedAppointmentDatesResult = result!.Appointments.Select(x => x.ScheduledFor).Order().ToList();
        for (int i = 0; i < orderedAppointmentDates.Count; i++)
        {
            orderedAppointmentDates[i].Should().BeCloseTo(orderedAppointmentDatesResult[i], TimeSpan.FromMilliseconds(100));
        }
        reservedTimeSlots.Should().BeEquivalentTo(result!.ReservedTimeSlots, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetSchedule_WhenAppointmentsHasValidService_ShouldReturnAppointmentsWithServiceDetails()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Service service = staffMember.Services.First();
        List<Appointment> appointments = await TestUtils.SetupAppointments(staffMember, Context);
        appointments.ForEach(x => x.ServiceId = service.Id);
        await Context.SaveChangesAsync();
        List<ReservedTimeSlot> reservedTimeSlots = await TestUtils.SetupReservedTimeSlots(staffMember, Context);
        string requestUri = $"{StaffMemberControllerConstants.GetScheduleUri}?staffMemberId={staffMember.Id}&until=2030-01-01";

        var result = await Client.GetFromJsonAsync<GetScheduleResponse>(requestUri);

        result.Should().NotBeNull();
        result!.Appointments.Select(x => x.ServiceId).Should().AllBeEquivalentTo(service.Id);
    }

    [Fact]
    public async Task GetById_WhenValidRequest_ShouldGetStaffMemberById()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        string requestUri = $"{StaffMemberControllerConstants.GetByIdUri}?id={staffMember.Id}";

        var result = await Client.GetFromJsonAsync<GetStaffMemberResponse>(requestUri);

        result.Should().NotBeNull();
        AssertValues(result!, staffMember);
    }

    [Fact]
    public async Task AddStaffMember_WhenValidRequest_ShouldAddStaffMember()
    {
        Business business = await TestUtils.SetupBusiness(Context);
        Guid userId = await TestUtils.SetupUser(Context);
        AddStaffMemberCommand request = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate() with
        {
            BusinessId = business.Id,
            UserId = userId,
        };
        var result = await Client.PostAsJsonAsync(StaffMemberControllerConstants.AddStaffMemberUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await result.Content.ReadFromJsonAsync<AddStaffMemberResponse>();
        response.Should().NotBeNull();
        var staffMemberFromDb = await Context.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == response!.StaffMemberId);
        staffMemberFromDb.Should().NotBeNull();
        AssertValues(request, staffMemberFromDb!);
    }

    [Fact]
    public async Task DeleteStaffMember_WhenValidRequest_ShouldDeleteStaffMember()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        staffMember.Position = PositionType.BusinessOwner;
        await Context.SaveChangesAsync();
        User? user = await Context.Users.FirstOrDefaultAsync(x => x.Id == staffMember.UserId);
        TestUtils.SetupUserTokenHeader(Client, user!.UserToken);
        string requestUri = $"{StaffMemberControllerConstants.DeleteStaffMemberUri}?staffMemberId={staffMember.Id}";

        var result = await Client.DeleteAsync(requestUri);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var staffMemberFromDb = await Context.StaffMembers.FirstOrDefaultAsync(x => x.Id == staffMember.Id);
        staffMemberFromDb.Should().BeNull();
        var users = await Context.Users.ToListAsync();
        users.Should().BeEmpty();
        var services = await Context.Services.Where(x => x.StaffMemberId == staffMember.Id).ToListAsync();
        services.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateStaffMemberSettings_WhenValidRequest_ShouldUpdateStaffMemberSettings()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var command = new UpdateStaffMemberSettingsCommand(staffMember.Id, new StaffMemberSettings(true, true));

        var result = await Client.PutAsJsonAsync(StaffMemberControllerConstants.UpdateStaffMemberSettingsUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITorDbContext>();
        var staffMemberFromDb = await context.StaffMembers.FirstOrDefaultAsync(x => x.Id == staffMember.Id);
        staffMemberFromDb!.Settings.Should().Be(command.Settings);
    }

    private void AssertValues(AddStaffMemberCommand request, StaffMember staffMemberFromDb)
    {
        request.BusinessId.Should().Be(staffMemberFromDb.BusinessId);
        request.Name.Should().Be(staffMemberFromDb.Name);
        request.Email.Should().Be(staffMemberFromDb.Email);
        request.PhoneNumber.Should().Be(staffMemberFromDb.PhoneNumber);
        staffMemberFromDb.ProfileImage.Should().Be(request.ProfileImage);
    }

    private static void AssertValues(GetStaffMemberResponse response, StaffMember staffMember)
    {
        response.Id.Should().Be(staffMember.Id);
        response.Name.Should().Be(staffMember.Name);
        response.Email.Should().Be(staffMember.Email);
        response.PhoneNumber.Should().Be(staffMember.PhoneNumber);
        response.BirthDate.Should().Be(staffMember.BirthDate);
        response.Position.Should().Be(staffMember.Position);
        response.Address.Should().Be(staffMember.Address);
        response.ProfileImage.Should().Be(staffMember.ProfileImage);
        response.Services.Select(x => x.Id).Should().BeEquivalentTo(staffMember.Services.Select(x => x.Id));
        response.Settings.Should().Be(staffMember.Settings);
    }
}
