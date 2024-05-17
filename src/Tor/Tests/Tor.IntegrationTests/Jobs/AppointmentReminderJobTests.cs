using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Common;
using Tor.Application.Common.Extensions;
using Tor.Infrastructure.Jobs.AppointmentReminder;
using Tor.TestsInfra;

namespace Tor.IntegrationTests.Jobs;

public class AppointmentReminderJobTests : BaseIntegrationTest
{
    private readonly IJobExecutionContext _executionContext;
    private readonly Mock<IPushNotificationSender> _pushNotificationSenderMock;
    private readonly AppointmentReminderJob _job;

    public AppointmentReminderJobTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
        _executionContext = new Mock<IJobExecutionContext>().Object;
        _pushNotificationSenderMock = factory.PushNotificationSenderMock;
        _pushNotificationSenderMock.Invocations.Clear();

        var scope = factory.Services.CreateScope();
        _job = new AppointmentReminderJob(
            scope.ServiceProvider.GetRequiredService<ILogger<AppointmentReminderJob>>(),
            scope.ServiceProvider.GetRequiredService<ISender>());
    }

    [Fact]
    public async Task Handle_WhenAppointmentsNeedsToBeReminded_ShouldNotifyClients()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var appointments = await TestUtils.SetupAppointments(staffMember, Context);
        appointments.ForEach(x =>
        {
            x.ScheduledFor = DateTimeOffset.UtcNow.AddHours(staffMember.Business.Settings.AppointmentReminderTimeInHours);
        });
        await Context.SaveChangesAsync();

        await _job.Execute(_executionContext);

        foreach (var appointment in appointments)
        {
            string message = AppointmentMessageBuilder.BuildAppointmentReminderMessage(
                appointment.ClientDetails.Name,
                appointment.ServiceDetails.Name,
                appointment.ScheduledFor.ToIsraelTime(),
                appointment.Notes);
            IEnumerable<string> deviceTokens = appointment.Client!.User.Devices.Select(x => x.Token);
            _pushNotificationSenderMock.Verify(x => x.Send(
                It.Is<SendPushNotificationRequest>(request =>
                    request.DeviceTokens.SequenceEqual(deviceTokens) &&
                    request.Title == appointment.StaffMember.Business.Name &&
                    request.Message == message), It.IsAny<CancellationToken>()),
                Times.Once);
            await Context.Entry(appointment).ReloadAsync();
            appointment.HasReceivedReminder.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Handle_WhenAppointmentsHaveBeenReminded_ShouldNotNotifyClients()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var appointments = await TestUtils.SetupAppointments(staffMember, Context);
        appointments.ForEach(x =>
        {
            x.HasReceivedReminder = true;
        });
        await Context.SaveChangesAsync();

        await _job.Execute(_executionContext);

        _pushNotificationSenderMock.Verify(x => x.Send(
            It.IsAny<SendPushNotificationRequest>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
