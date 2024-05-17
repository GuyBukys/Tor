using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Tor.Application;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Businesses.Notifications.BusinessCreated;
using Tor.Application.Common.Settings;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class BusinessCreatedNotificationHandlerTests : UnitTestBase
{
    private readonly BusinessCreatedNotificationHandler _sut;
    private readonly Mock<IPushNotificationSender> _mockPushNotificationSender;
    private readonly Mock<IOptions<TwoKingsSettings>> _mockSettings;

    public BusinessCreatedNotificationHandlerTests()
        : base()
    {
        _mockPushNotificationSender = new Mock<IPushNotificationSender>();
        _mockSettings = new Mock<IOptions<TwoKingsSettings>>();
        _mockSettings.Setup(x => x.Value).Returns(new TwoKingsSettings { Devices = ["token1", "token2"] });

        _sut = new BusinessCreatedNotificationHandler(
            Context,
            _mockPushNotificationSender.Object,
            _mockSettings.Object,
            new NullLogger<BusinessCreatedNotificationHandler>());
    }

    [Fact]
    public async Task Handle_WhenValidNotification_ShouldAddBusinessToGroupAndSendWelcomeMessage()
    {
        var notification = Fakers.Businesses.BusinessCreatedNotificationFaker.Generate();

        await _sut.Handle(notification, default);

        _mockPushNotificationSender.Verify(x =>
            x.AddToGroup(It.Is<AddToGroupRequest>(request =>
                request.GroupName == Constants.Groups.BusinessesNotificationGroup &&
                request.DeviceTokens.SequenceEqual(notification.Devices.Select(x => x.Token)))),
            Times.Once());
        _mockPushNotificationSender.Verify(x =>
            x.Send(It.Is<SendPushNotificationRequest>(request =>
                request.DeviceTokens.SequenceEqual(_mockSettings.Object.Value.Devices) &&
                request.Title == "Cha Ching"),
            It.IsAny<CancellationToken>()),
            Times.Once());
    }
}
