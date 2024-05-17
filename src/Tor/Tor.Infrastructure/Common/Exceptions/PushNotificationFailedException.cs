namespace Tor.Infrastructure.Common.Exceptions;

internal class PushNotificationFailedException : Exception
{
    internal PushNotificationFailedException(string message)
        : base(message)
    {
    }
}
