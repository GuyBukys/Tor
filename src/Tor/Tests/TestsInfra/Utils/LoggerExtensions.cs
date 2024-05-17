using Microsoft.Extensions.Logging;
using Moq;

namespace Tor.TestsInfra.Utils;
internal static class LoggerExtensions
{
    internal static void VerifyLogContains<T>(
        this Mock<ILogger<T>> _loggerMock,
        string message,
        LogLevel logLevel = LogLevel.Information)
    {
        _loggerMock.Verify(
            logger => logger.Log(
                logLevel,
                0,
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
