using Microsoft.Extensions.Logging;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.StaffMembers.Notifications.StaffMemberAdded;
using Tor.TestsInfra;

namespace Tor.UnitTests.Application.StaffMembers;

public class StaffMemberAddedNotificationHandlerTests : UnitTestBase
{
    private readonly Mock<IPushNotificationSender> _mockPushNotificationSender;
    private readonly Mock<ILogger<StaffMemberAddedNotificationHandler>> _mockLogger;

    private readonly StaffMemberAddedNotificationHandler _sut;

    public StaffMemberAddedNotificationHandlerTests()
        : base()
    {
        _mockPushNotificationSender = new Mock<IPushNotificationSender>();
        _mockLogger = new Mock<ILogger<StaffMemberAddedNotificationHandler>>();

        _sut = new StaffMemberAddedNotificationHandler(Context, _mockPushNotificationSender.Object, _mockLogger.Object);
    }
}
