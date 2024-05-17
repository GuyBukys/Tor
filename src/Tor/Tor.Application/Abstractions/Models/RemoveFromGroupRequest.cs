namespace Tor.Application.Abstractions.Models;

public record RemoveFromGroupRequest(
    IEnumerable<string> DeviceTokens,
    string GroupName);
