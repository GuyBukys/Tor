namespace Tor.Application.Abstractions.Models;

public record SendToGroupRequest(
    string GroupName,
    string Title,
    string Message);
