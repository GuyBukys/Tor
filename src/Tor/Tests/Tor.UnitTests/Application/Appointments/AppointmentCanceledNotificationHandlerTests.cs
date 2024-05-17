using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Common;
using Tor.Application.Appointments.Notifications.AppointmentCanceled;
using Tor.Application.Common.Extensions;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;
using Tor.Domain.UserAggregate;
using Tor.Domain.WaitingListAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Appointments;
public class AppointmentCanceledNotificationHandlerTests : UnitTestBase
{
    private readonly Mock<IPushNotificationSender> _pushNotificationSenderMock;
    private readonly AppointmentCanceledNotificationHandler _sut;

    public AppointmentCanceledNotificationHandlerTests()
        : base()
    {
        _pushNotificationSenderMock = new Mock<IPushNotificationSender>();

        _sut = new AppointmentCanceledNotificationHandler(Context, _pushNotificationSenderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenNotifyWaitingListIsTrueAndThereAreClientsInWaitingList_ShouldSendNotificationToClients()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        WaitingList waitingList = Fakers.WaitingLists.WaitingListFaker.Generate();
        List<User> users = [];
        waitingList.Clients.ForEach(async client =>
        {
            User user = Fakers.Users.UserFaker.Generate();
            user.EntityId = client.Id;
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            users.Add(user);
        });
        waitingList.StaffMemberId = appointment.StaffMemberId;
        waitingList.AtDate = DateOnly.FromDateTime(appointment.ScheduledFor.ToIsraelTime());
        await Context.Appointments.AddAsync(appointment);
        await Context.WaitingLists.AddAsync(waitingList);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var notification = Fakers.Appointments.AppointmentCanceledNotificationFaker.Generate() with
        {
            StaffMemberId = appointment.StaffMemberId,
            BusinessId = business.Id,
            ServiceName = appointment.ServiceDetails.Name,
            AppointmentScheduledDate = appointment.ScheduledFor,
            NotifyWaitingList = true,
        };

        await _sut.Handle(notification, default);

        var deviceTokens = users.SelectMany(x => x.Devices).Select(x => x.Token);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x =>
                x.DeviceTokens.SequenceEqual(deviceTokens) &&
                x.Title == business.Name &&
                x.Message == AppointmentMessageBuilder.BuildWaitingListMessage(notification.ServiceName, DateOnly.FromDateTime(appointment.ScheduledFor.ToIsraelTime()))),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_WhenNotifyClientIsTrueAndClientIdIsNotNull_ShouldSendNotificationToClient()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        Client client = Fakers.Clients.ClientFaker.Generate();
        User user = Fakers.Users.UserFaker.Generate();
        user.EntityId = client.Id;
        appointment.ClientId = client.Id;
        await Context.Appointments.AddAsync(appointment);
        await Context.Clients.AddAsync(client);
        await Context.Businesses.AddAsync(business);
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var notification = Fakers.Appointments.AppointmentCanceledNotificationFaker.Generate() with
        {
            StaffMemberId = appointment.StaffMemberId,
            ServiceName = appointment.ServiceDetails.Name,
            BusinessId = business.Id,
            ClientId = client.Id,
            AppointmentScheduledDate = appointment.ScheduledFor,
            NotifyClient = true,
        };

        await _sut.Handle(notification, default);

        var deviceTokens = user.Devices.Select(x => x.Token);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x =>
                x.DeviceTokens.SequenceEqual(deviceTokens) &&
                x.Title == business.Name &&
                x.Message == AppointmentMessageBuilder.BuildClientAppointmentCanceledMessage(appointment.ScheduledFor, notification.ServiceName, notification.Reason)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotifyClientIsFalse_ShouldNotSendNotificationToClient()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        Client client = Fakers.Clients.ClientFaker.Generate();
        appointment.ClientId = client.Id;
        await Context.Appointments.AddAsync(appointment);
        await Context.Clients.AddAsync(client);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var notification = Fakers.Appointments.AppointmentCanceledNotificationFaker.Generate() with
        {
            BusinessId = business.Id,
            ServiceName = appointment.ServiceDetails.Name,
            StaffMemberId = appointment.StaffMemberId,
            ClientId = client.Id,
            AppointmentScheduledDate = appointment.ScheduledFor,
            NotifyClient = false,
        };

        await _sut.Handle(notification, default);

        var deviceTokens = client.User!.Devices.Select(x => x.Token);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x => x.DeviceTokens.SequenceEqual(deviceTokens)),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [Fact]
    public async Task Handle_WhenNotifyStaffMemberIsTrue_ShouldNotifyStaffMember()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        Service service = staffMember.Services.First();
        User user = Fakers.Users.UserFaker.Generate();
        staffMember.User = user;
        user.EntityId = staffMember.Id;
        business.StaffMembers.Add(staffMember);
        Client client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var notification = Fakers.Appointments.AppointmentCanceledNotificationFaker.Generate() with
        {
            BusinessId = business.Id,
            StaffMemberId = staffMember.Id,
            ServiceName = service.Name,
            ClientId = client.Id,
            AppointmentScheduledDate = DateTimeOffset.UtcNow,
            NotifyStaffMember = true,
        };

        await _sut.Handle(notification, default);

        var staffMemberDeviceTokens = staffMember.User.Devices.Select(x => x.Token);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x => x.DeviceTokens.SequenceEqual(staffMemberDeviceTokens)),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_WhenNotifyWaitingListIsFalse_ShouldNotSendNotificationToClientsInWaitingList()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        WaitingList waitingList = Fakers.WaitingLists.WaitingListFaker.Generate();
        waitingList.StaffMemberId = appointment.StaffMemberId;
        waitingList.AtDate = DateOnly.FromDateTime(appointment.ScheduledFor.ToIsraelTime());
        await Context.Appointments.AddAsync(appointment);
        await Context.WaitingLists.AddAsync(waitingList);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var notification = Fakers.Appointments.AppointmentCanceledNotificationFaker.Generate() with
        {
            BusinessId = business.Id,
            StaffMemberId = appointment.StaffMemberId,
            ServiceName = appointment.ServiceDetails.Name,
            AppointmentScheduledDate = appointment.ScheduledFor,
            NotifyWaitingList = false,
        };

        await _sut.Handle(notification, default);

        var deviceTokens = waitingList.Clients.SelectMany(x => x.User!.Devices).Select(x => x.Token);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(x => x.DeviceTokens.SequenceEqual(deviceTokens)),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [Fact]
    public async Task Handle_WhenNotifyWaitingListIsTrueAndThereIsNoWaitingList_ShouldNotSend()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        List<WaitingList> waitingLists = Enumerable.Range(1, 5).Select(_ =>
        {
            var waitingList = Fakers.WaitingLists.WaitingListFaker.Generate();
            waitingList.StaffMemberId = appointment.StaffMemberId;
            waitingList.AtDate = DateOnly.FromDateTime(appointment.ScheduledFor.ToIsraelTime().AddDays(1));
            return waitingList;
        }).ToList();
        await Context.Appointments.AddAsync(appointment);
        await Context.WaitingLists.AddRangeAsync(waitingLists);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var notification = Fakers.Appointments.AppointmentCanceledNotificationFaker.Generate() with
        {
            BusinessId = business.Id,
            ServiceName = appointment.ServiceDetails.Name,
            StaffMemberId = appointment.StaffMemberId,
            AppointmentScheduledDate = appointment.ScheduledFor,
            NotifyWaitingList = true,
        };

        await _sut.Handle(notification, default);

        _pushNotificationSenderMock.Verify(x => x.Send(
            It.IsAny<SendPushNotificationRequest>(),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [Fact]
    public async Task Handle_WhenNotifyWaitingListIsTrueAndThereAreNoClientsInWaitingList_ShouldNotSend()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        await Context.Appointments.AddAsync(appointment);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var notification = Fakers.Appointments.AppointmentCanceledNotificationFaker.Generate() with
        {
            BusinessId = business.Id,
            ServiceName = appointment.ServiceDetails.Name,
            StaffMemberId = appointment.StaffMemberId,
            AppointmentScheduledDate = appointment.ScheduledFor,
            NotifyWaitingList = true,
        };

        await _sut.Handle(notification, default);

        _pushNotificationSenderMock.Verify(x => x.Send(
            It.IsAny<SendPushNotificationRequest>(),
            It.IsAny<CancellationToken>()),
            Times.Never());
    }
}
