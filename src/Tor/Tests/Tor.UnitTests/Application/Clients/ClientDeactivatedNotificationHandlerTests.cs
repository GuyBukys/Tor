using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Clients.Notifications.ClientDeactivated;
using Tor.TestsInfra;

namespace Tor.UnitTests.Application.Clients;

public class ClientDeactivatedNotificationHandlerTests : UnitTestBase
{
    private readonly Mock<IPushNotificationSender> _pushNotificationSenderMock;
    private readonly ClientDeactivatedNotificationHandler _sut;

    public ClientDeactivatedNotificationHandlerTests()
    {
        _pushNotificationSenderMock = new Mock<IPushNotificationSender>();

        _sut = new ClientDeactivatedNotificationHandler(Context, _pushNotificationSenderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidNotification_ShouldSendGoodbyeMessageAndRemoveFromGroup()
    {
        var notification = new ClientDeactivatedNotification(Enumerable.Range(1, 5).Select(_ => Guid.NewGuid().ToString()).ToList());

        await _sut.Handle(notification, default);

        _pushNotificationSenderMock.Verify(x =>
            x.Send(
                It.Is<SendPushNotificationRequest>(x => x.DeviceTokens.SequenceEqual(notification.DeviceTokens)),
                It.IsAny<CancellationToken>()),
                Times.Once);
        _pushNotificationSenderMock.Verify(x =>
            x.RemoveFromGroup(
                It.Is<RemoveFromGroupRequest>(x =>
                    x.DeviceTokens.SequenceEqual(notification.DeviceTokens) &&
                    x.GroupName == Tor.Application.Constants.Groups.ClientsNotificationGroup)),
                Times.Once);
    }
}
