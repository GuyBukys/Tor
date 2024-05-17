namespace Tor.Application.Abstractions.Models;

public record SendPushNotificationRequest(
    IEnumerable<string> DeviceTokens,
    string Title,
    string Message,
    IReadOnlyDictionary<string, string>? Metadata = null);
