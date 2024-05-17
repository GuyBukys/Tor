namespace Tor.Application.Abstractions.Models;

public record AddToGroupRequest(
    IEnumerable<string> DeviceTokens,
    string GroupName);

