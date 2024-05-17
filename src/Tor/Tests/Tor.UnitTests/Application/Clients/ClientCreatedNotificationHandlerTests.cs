using Moq;
using Tor.Application;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Clients.Notifications.ClientCreated;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class ClientCreatedNotificationHandlerTests : UnitTestBase
{
    private readonly Mock<IPushNotificationSender> _mockPushNotificationSender;
    private readonly ClientCreatedNotificationHandler _sut;

    public ClientCreatedNotificationHandlerTests()
        : base()
    {
        _mockPushNotificationSender = new Mock<IPushNotificationSender>();
        _sut = new ClientCreatedNotificationHandler(_mockPushNotificationSender.Object);
    }

    [Fact]
    public async Task Handle_WhenValidNotification_ShouldAddClientToGroup()
    {
        var notification = Fakers.Clients.ClientCreatedNotificationFaker.Generate();

        await _sut.Handle(notification, default);

        _mockPushNotificationSender.Verify(x =>
            x.AddToGroup(It.Is<AddToGroupRequest>(request =>
                request.GroupName == Constants.Groups.ClientsNotificationGroup &&
                request.DeviceTokens.SequenceEqual(notification.Devices.Select(x => x.Token)))),
            Times.Once());
    }
}
