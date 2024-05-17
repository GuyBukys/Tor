using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Tor.Application;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;
using Tor.Domain.MessageBlastAggregate.Entities;
using Tor.Infrastructure.Jobs.ScheduleAppointmentReminder;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.Jobs;

public class ScheduleAppointmentReminderJobTests : BaseIntegrationTest
{
    private readonly IJobExecutionContext _executionContext;
    private readonly Mock<IPushNotificationSender> _mockPushNotificationSender;
    private readonly ScheduleAppointmentReminderJob _job;

    public ScheduleAppointmentReminderJobTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
        _executionContext = new Mock<IJobExecutionContext>().Object;
        _mockPushNotificationSender = factory.PushNotificationSenderMock;

        var scope = factory.Services.CreateScope();
        _job = new ScheduleAppointmentReminderJob(
            scope.ServiceProvider.GetRequiredService<ILogger<ScheduleAppointmentReminderJob>>(),
            scope.ServiceProvider.GetRequiredService<ISender>());
    }

    [Fact]
    public async Task Execute_WhenClientsLastAppointmentIsExactlyOneMonthAgo_ShouldGetNotified()
    {
        Business business = await TestUtils.SetupBusiness(Context);
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        business.StaffMembers.Add(staffMember);
        Client client = await TestUtils.SetupClient(Context);
        business.Clients.Add(client);
        await Context.SaveChangesAsync();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        appointment.StaffMemberId = staffMember.Id;
        appointment.ClientId = client.Id;
        appointment.ScheduledFor = DateTime.UtcNow.AddDays(-30);
        await Context.Appointments.AddAsync(appointment);
        await Context.SaveChangesAsync();
        BusinessMessageBlast businessMessageBlast = Fakers.MessageBlasts.BusinessMessageBlastFaker.Generate();
        Guid messageBlastId = await Context.MessageBlasts
            .Where(x => x.Name == Constants.MessageBlasts.ScheduleAppointmentReminderName)
            .Select(x => x.Id)
            .FirstAsync();
        businessMessageBlast.BusinessId = business.Id;
        businessMessageBlast.MessageBlastId = messageBlastId;
        await Context.BusinessMessageBlasts.AddAsync(businessMessageBlast);
        await Context.SaveChangesAsync();

        await _job.Execute(_executionContext);

        _mockPushNotificationSender.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(request => request.DeviceTokens.SequenceEqual(client.User!.Devices.Select(x => x.Token))),
            It.IsAny<CancellationToken>()));
    }
}
