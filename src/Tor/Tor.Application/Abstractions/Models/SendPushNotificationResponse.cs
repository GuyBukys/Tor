namespace Tor.Application.Abstractions.Models;

public class SendPushNotificationResponse
{
    public IEnumerable<string> MessageIds { get; set; } = [];
}
