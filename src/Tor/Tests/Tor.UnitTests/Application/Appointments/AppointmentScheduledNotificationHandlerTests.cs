using FluentAssertions;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Common;
using Tor.Application.Appointments.Notifications.AppointmentScheduled;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;
using Tor.Domain.UserAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Appointments;
public class AppointmentScheduledNotificationHandlerTests : UnitTestBase
{
    private readonly Mock<IPushNotificationSender> _pushNotificationSenderMock;
    private readonly AppointmentScheduledNotificationHandler _sut;

    public AppointmentScheduledNotificationHandlerTests()
        : base()
    {
        _pushNotificationSenderMock = new Mock<IPushNotificationSender>();

        _sut = new AppointmentScheduledNotificationHandler(Context, _pushNotificationSenderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenClientIdIsNotNullAndNotifyClientTrue_ShouldAddClientToBusinessSendNotificationToClient()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        Client client = Fakers.Clients.ClientFaker.Generate();
        User user = Fakers.Users.UserFaker.Generate();
        user.EntityId = client.Id;
        await Context.Clients.AddAsync(client);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var notification = new AppointmentScheduledNotification(client.Id, Guid.NewGuid(), client.Name, business.Id, DateTimeOffset.UtcNow, true, false);

        await _sut.Handle(notification, default);

        await Context.Entry(business).ReloadAsync();
        business.Clients.Should().Contain(x => x.Id == client.Id);
        var deviceTokens = user.Devices.Select(x => x.Token);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x =>
                x.DeviceTokens.SequenceEqual(deviceTokens) &&
                x.Title == business.Name &&
                x.Message == AppointmentMessageBuilder.BuildClientAppointmentScheduledMessage(business.Name, notification.ScheduledFor, business.NotesForAppointment)),
            It.IsAny<CancellationToken>()),
            Times.Once());
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x =>
                x.Message == AppointmentMessageBuilder.BuildStaffMemberAppointmentCanceledMessage(notification.ScheduledFor, notification.ClientName)),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [Fact]
    public async Task Handle_WhenClientIdIsNull_ShouldDoNothing()
    {
        var notification = new AppointmentScheduledNotification(null, Guid.NewGuid(), "name", Guid.NewGuid(), DateTimeOffset.UtcNow, false, false);

        await _sut.Handle(notification, default);

        _pushNotificationSenderMock.Verify(x => x.Send(
            It.IsAny<SendPushNotificationRequest>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNotifyClientIsFalse_ShouldNotNotifyClient()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        Client client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        var notification = new AppointmentScheduledNotification(client.Id, Guid.NewGuid(), client.Name, business.Id, DateTimeOffset.UtcNow, NotifyClient: false, false);

        await _sut.Handle(notification, default);

        await Context.Entry(business).ReloadAsync();
        business.Clients.Should().Contain(x => x.Id == client.Id);
        var deviceTokens = client.User!.Devices.ConvertAll(x => x.Token);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.IsAny<SendPushNotificationRequest>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNotifyStaffMemberIsTrue_ShouldNotifyStaffMember()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        User user = Fakers.Users.UserFaker.Generate();
        user.EntityId = staffMember.Id;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        Client client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        var notification = new AppointmentScheduledNotification(client.Id, staffMember.Id, client.Name, business.Id, DateTimeOffset.UtcNow, false, true);

        await _sut.Handle(notification, default);

        await Context.Entry(business).ReloadAsync();
        business.Clients.Should().Contain(x => x.Id == client.Id);
        var deviceTokens = user.Devices.ConvertAll(x => x.Token);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x =>
                x.DeviceTokens.SequenceEqual(deviceTokens) &&
                x.Message == AppointmentMessageBuilder.BuildStaffMemberAppointmentScheduledMessage(notification.ScheduledFor, notification.ClientName)),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }
}
