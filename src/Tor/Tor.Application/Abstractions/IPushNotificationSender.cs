using Tor.Application.Abstractions.Models;

namespace Tor.Application.Abstractions;

public interface IPushNotificationSender
{
    Task<SendPushNotificationResponse> Send(SendPushNotificationRequest request, CancellationToken cancellationToken);
    Task<AddToGroupResponse> AddToGroup(AddToGroupRequest request);
    Task<RemoveFromGroupResponse> RemoveFromGroup(RemoveFromGroupRequest request);
    Task<SendToGroupResponse> SendToGroup(SendToGroupRequest request, CancellationToken cancellationToken);
}
