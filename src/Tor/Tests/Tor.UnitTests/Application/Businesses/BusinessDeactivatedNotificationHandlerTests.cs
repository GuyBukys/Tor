using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Businesses.Notifications.BusinessDeactivated;
using Tor.TestsInfra;

namespace Tor.UnitTests.Application.Businesses;

public class BusinessDeactivatedNotificationHandlerTests : UnitTestBase
{
    private readonly Mock<IPushNotificationSender> _pushNotificationSenderMock;
    private readonly BusinessDeactivatedNotificationHandler _sut;

    public BusinessDeactivatedNotificationHandlerTests()
    {
        _pushNotificationSenderMock = new Mock<IPushNotificationSender>();

        _sut = new BusinessDeactivatedNotificationHandler(_pushNotificationSenderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidNotification_ShouldRemoveFromGroup()
    {
        var notification = new BusinessDeactivatedNotification(Enumerable.Range(1, 5).Select(_ => Guid.NewGuid().ToString()).ToList());

        await _sut.Handle(notification, default);

        _pushNotificationSenderMock.Verify(x =>
            x.RemoveFromGroup(
                It.Is<RemoveFromGroupRequest>(x =>
                    x.DeviceTokens.SequenceEqual(notification.DeviceTokens) &&
                    x.GroupName == Tor.Application.Constants.Groups.BusinessesNotificationGroup)),
                Times.Once);
    }
}
