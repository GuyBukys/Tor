using FluentAssertions;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Commands.ApproveAppointment;
using Tor.Application.Appointments.Commands.CancelAppointment;
using Tor.Application.Common.Extensions;
using Tor.Contracts.Appointment;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.ClientAggregate;
using Tor.Domain.WaitingListAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.Appointments;

public class AppointmentControllerTests : BaseIntegrationTest
{
    private readonly Mock<IPushNotificationSender> _pushNotificationSenderMock;
    private readonly Mock<IIsraelDateTimeProvider> _dateTimeProviderMock;
    private readonly IntegrationTestWebApplicationFactory _factory;

    public AppointmentControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
        _pushNotificationSenderMock = factory.PushNotificationSenderMock;
        _dateTimeProviderMock = factory.IsraelDateTimeProviderMock;
        _factory = factory;
    }

    [Fact]
    public async Task CancelAppointment_WhenValidRequest_ShouldCancelAppointment()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        List<Appointment> appointments = await TestUtils.SetupAppointments(staffMember, Context);
        Appointment appointment = appointments.First();
        WaitingList waitingList = Fakers.WaitingLists.WaitingListFaker.Generate();
        waitingList.AtDate = DateOnly.FromDateTime(appointment.ScheduledFor.UtcDateTime);
        waitingList.StaffMemberId = appointment.StaffMemberId;
        await Context.WaitingLists.AddAsync(waitingList);
        await Context.SaveChangesAsync();
        CancelAppointmentCommand request = new(appointment.Id, "reason");

        var result = await Client.PutAsJsonAsync(AppointmentControllerConstants.CancelAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(appointment).ReloadAsync();
        appointment.Status.Should().Be(AppointmentStatusType.Canceled);
    }

    [Fact]
    public async Task GetAvailableTimes_WhenValidRequest_ShouldGetAvailableTimes()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Service service = await TestUtils.SetupService(staffMember.Id, Context);
        string requestUri = $"{AppointmentControllerConstants.GetAvailableTimesUri}" +
            $"?serviceId={service.Id}" +
            $"&startDate={DateOnly.FromDateTime(DateTime.UtcNow):yyyy-MM-dd}" +
            $"&endDate={DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)):yyyy-MM-dd}";

        var result = await Client.GetFromJsonAsync<GetAvailableTimesResponse>(requestUri);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ScheduleAppointment_WhenValidRequestRegular_ShouldCreateAppointmentAndNotifyClient()
    {
        var scheduledFor = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        _dateTimeProviderMock.Setup(x => x.Now).Returns(scheduledFor.AddHours(-1).DateTime);
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Client client = await TestUtils.SetupClient(Context);
        await Context.SaveChangesAsync();
        var request = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            ClientId = client.Id,
            Type = AppointmentType.Regular,
            ScheduledFor = scheduledFor,
            NotifyClient = true,
        };
        request.ServiceDetails.Durations = [new Duration(1, 1, DurationType.Work)];
        await SetupSchedule(staffMember.WeeklySchedule, scheduledFor.DateTime);

        var result = await Client.PostAsJsonAsync(AppointmentControllerConstants.ScheduleAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await result.Content.ReadFromJsonAsync<ScheduleAppointmentResponse>();
        response.Should().NotBeNull();
        var appointment = await Context.Appointments.SingleOrDefaultAsync(x => x.Id == response!.AppointmentId);
        appointment.Should().NotBeNull();
        appointment!.ScheduledFor.Should().BeCloseTo(request.ScheduledFor, TimeSpan.FromMilliseconds(100));
        appointment!.Status.Should().Be(AppointmentStatusType.Approved);
        bool isClientExistInBusinessList = await Context.BusinessClients.AnyAsync(x =>
            x.BusinessId == staffMember.BusinessId && x.ClientId == client.Id);
        isClientExistInBusinessList.Should().BeTrue();
        var deviceTokens = client.User.Devices
            .Select(x => x.Token)
            .ToList();
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x => x.DeviceTokens.SequenceEqual(deviceTokens)),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task ScheduleAppointment_WhenValidRequestAndHasServiceId_ShouldCreateAppointmentByService()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Service service = staffMember.Services.First();
        Client client = await TestUtils.SetupClient(Context);
        await Context.SaveChangesAsync();
        var request = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            ClientId = client.Id,
            Type = AppointmentType.Manual,
            ServiceId = service.Id,
            NotifyClient = false,
        };

        var result = await Client.PostAsJsonAsync(AppointmentControllerConstants.ScheduleAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await result.Content.ReadFromJsonAsync<ScheduleAppointmentResponse>();
        response.Should().NotBeNull();
        var appointment = await Context.Appointments.SingleOrDefaultAsync(x => x.Id == response!.AppointmentId);
        appointment.Should().NotBeNull();
        appointment!.ScheduledFor.Should().BeCloseTo(request.ScheduledFor, TimeSpan.FromMilliseconds(100));
        appointment!.Status.Should().Be(AppointmentStatusType.Created);
        appointment!.ServiceDetails.Name.Should().Be(service.Name);
        appointment!.ServiceDetails.Description.Should().Be(service.Description);
        appointment!.ServiceDetails.Amount.Should().Be(service.Amount);
        appointment!.ServiceDetails.Durations.Should().BeEquivalentTo(service.Durations);
    }

    [Fact]
    public async Task RescheduleAppointment_WhenValidRequestAndHasServiceId_ShouldCreateAppointmentByService()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Service service = staffMember.Services.First();
        Client client = await TestUtils.SetupClient(Context);
        var appointments = await TestUtils.SetupAppointments(staffMember, Context, client.Id);
        var appointmentToReschedule = appointments.First();
        await Context.SaveChangesAsync();
        var request = Fakers.Appointments.RescheduleAppointmentCommandFaker.Generate() with
        {
            AppointmentId = appointmentToReschedule.Id,
            StaffMemberId = staffMember.Id,
            ServiceId = service.Id,
        };

        var result = await Client.PutAsJsonAsync(AppointmentControllerConstants.RescheduleAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await result.Content.ReadFromJsonAsync<ScheduleAppointmentResponse>();
        response.Should().NotBeNull();
        var newAppointmentFromDb = await Context.Appointments.FirstOrDefaultAsync(x => x.Id == response!.AppointmentId);
        newAppointmentFromDb!.ScheduledFor.Should().BeCloseTo(request.ScheduledFor, TimeSpan.FromMilliseconds(100));
        newAppointmentFromDb!.ServiceDetails.Name.Should().Be(service.Name);
        newAppointmentFromDb!.ServiceDetails.Description.Should().Be(service.Description);
        newAppointmentFromDb!.ServiceDetails.Amount.Should().Be(service.Amount);
        newAppointmentFromDb!.ServiceDetails.Durations.Should().BeEquivalentTo(service.Durations);
    }

    [Fact]
    public async Task ApproveAppointment_WhenValidRequest_ShouldApproveAppointment()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Service service = staffMember.Services.First();
        Client client = await TestUtils.SetupClient(Context);
        var appointments = await TestUtils.SetupAppointments(staffMember, Context, client.Id);
        Appointment appointment = appointments.First();
        ApproveAppointmentCommand request = new(appointment.Id);

        var result = await Client.PutAsJsonAsync(AppointmentControllerConstants.ApproveAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(appointment).ReloadAsync();
        appointment.Status.Should().Be(AppointmentStatusType.Approved);
    }

    [Fact]
    public async Task RescheduleAppointment_WhenValidRequest_ShouldCreateNewAppointment()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Client client = await TestUtils.SetupClient(Context);
        var appointments = await TestUtils.SetupAppointments(staffMember, Context, client.Id);
        var appointmentToReschedule = appointments.First();
        await Context.SaveChangesAsync();
        var request = Fakers.Appointments.RescheduleAppointmentCommandFaker.Generate() with
        {
            AppointmentId = appointmentToReschedule.Id,
            StaffMemberId = staffMember.Id,
        };

        var result = await Client.PutAsJsonAsync(AppointmentControllerConstants.RescheduleAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await result.Content.ReadFromJsonAsync<ScheduleAppointmentResponse>();
        response.Should().NotBeNull();
        var newAppointmentFromDb = await Context.Appointments.FirstOrDefaultAsync(x => x.Id == response!.AppointmentId);
        newAppointmentFromDb!.ScheduledFor.Should().BeCloseTo(request.ScheduledFor, TimeSpan.FromMilliseconds(100));
        newAppointmentFromDb!.Should().BeEquivalentTo(request, cfg =>
        {
            return cfg
            .Excluding(f => f.ScheduledFor)
            .ExcludingMissingMembers();
        });
    }

    [Fact]
    public async Task ScheduleAppointment_WhenRegularAppointmentAndCantAcquireLock_ShouldNotScheduleAppointment()
    {
        var scheduledFor = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        _dateTimeProviderMock.Setup(x => x.Now).Returns(scheduledFor.AddHours(-1).DateTime);
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Client client = await TestUtils.SetupClient(Context);
        var userId = await TestUtils.SetupUser(Context);
        client.UserId = userId;
        await Context.SaveChangesAsync();
        var request = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            ClientId = client.Id,
            Type = AppointmentType.Regular,
            ScheduledFor = scheduledFor,
            NotifyClient = true,
        };
        request.ServiceDetails.Durations = [new Duration(1, 1, DurationType.Work)];
        await SetupSchedule(staffMember.WeeklySchedule, scheduledFor.DateTime);
        var lockProvider = _factory.Services.GetRequiredService<IDistributedLockProvider>();

        IDistributedLock @lock = lockProvider.CreateLock($"ScheduleAppointment:{request.StaffMemberId}:{DateOnly.FromDateTime(request.ScheduledFor.UtcDateTime)}");
        await using var handle = await @lock.AcquireAsync();
        var result = await Client.PostAsJsonAsync(AppointmentControllerConstants.ScheduleAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var appointment = await Context.Appointments.FirstOrDefaultAsync();
        appointment.Should().BeNull();
        string content = await result.Content.ReadAsStringAsync();
        content.Should().Contain("could not acquire distributed lock");
    }

    [Fact]
    public async Task ScheduleAppointment_WhenRegularAppointmentClientIsBlocked_ShouldNotScheduleAppointment()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Client client = await TestUtils.SetupClient(Context);
        var userId = await TestUtils.SetupUser(Context);
        client.UserId = userId;
        await Context.SaveChangesAsync();
        await Context.BlockedClients.AddAsync(new BlockedClient { BusinessId = staffMember.BusinessId, ClientId = client.Id });
        await Context.SaveChangesAsync();
        var request = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            ClientId = client.Id,
            Type = AppointmentType.Regular,
        };

        var result = await Client.PostAsJsonAsync(AppointmentControllerConstants.ScheduleAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ScheduleAppointment_WhenValidRequestManual_ShouldOnlyCreateAppointment()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Client client = await TestUtils.SetupClient(Context);
        var userId = await TestUtils.SetupUser(Context);
        client.UserId = userId;
        await Context.SaveChangesAsync();
        var request = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            ClientId = client.Id,
            Type = AppointmentType.Manual,
        };

        var result = await Client.PostAsJsonAsync(AppointmentControllerConstants.ScheduleAppointmentUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await result.Content.ReadFromJsonAsync<ScheduleAppointmentResponse>();
        response.Should().NotBeNull();
        var appointment = await Context.Appointments.SingleOrDefaultAsync(x => x.Id == response!.AppointmentId);
        appointment.Should().NotBeNull();
        appointment!.ScheduledFor.Should().BeCloseTo(request.ScheduledFor, TimeSpan.FromMilliseconds(100));
        appointment!.Status.Should().Be(AppointmentStatusType.Created);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.IsAny<SendPushNotificationRequest>(),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    private async Task SetupSchedule(WeeklySchedule weeklySchedule, DateTime scheduledFor)
    {
        weeklySchedule.GetDailySchedule(scheduledFor.DayOfWeek).TimeRange = new(new(6, 0), new(23, 0));
        weeklySchedule.GetDailySchedule(scheduledFor.DayOfWeek).RecurringBreaks.Clear();

        await Context.SaveChangesAsync();
    }
}
