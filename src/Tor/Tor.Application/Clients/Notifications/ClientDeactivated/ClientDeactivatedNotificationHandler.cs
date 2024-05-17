using MediatR;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;

namespace Tor.Application.Clients.Notifications.ClientDeactivated;

internal sealed class ClientDeactivatedNotificationHandler : INotificationHandler<ClientDeactivatedNotification>
{
    private readonly IPushNotificationSender _pushNotificationSender;

    public ClientDeactivatedNotificationHandler(ITorDbContext context, IPushNotificationSender pushNotificationSender)
    {
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(ClientDeactivatedNotification notification, CancellationToken cancellationToken)
    {
        await SendGoodbyeMessage(notification.DeviceTokens, cancellationToken);
        await RemoveFromGroup(notification.DeviceTokens);
    }

    private async Task SendGoodbyeMessage(List<string> deviceTokens, CancellationToken cancellationToken)
    {
        SendPushNotificationRequest request = new(
           deviceTokens,
           Constants.ServiceName,
           "Sorry to see you go :(");

        await _pushNotificationSender.Send(request, cancellationToken);
    }

    private async Task RemoveFromGroup(List<string> deviceTokens)
    {
        RemoveFromGroupRequest request = new(
            deviceTokens,
            Constants.Groups.ClientsNotificationGroup);

        await _pushNotificationSender.RemoveFromGroup(request);
    }
}
