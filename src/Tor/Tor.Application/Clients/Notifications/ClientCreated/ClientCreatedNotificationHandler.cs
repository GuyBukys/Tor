using MediatR;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;

namespace Tor.Application.Clients.Notifications.ClientCreated;

internal sealed class ClientCreatedNotificationHandler : INotificationHandler<ClientCreatedNotification>
{
    private readonly IPushNotificationSender _pushNotificationSender;

    public ClientCreatedNotificationHandler(IPushNotificationSender pushNotificationSender)
    {
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(ClientCreatedNotification notification, CancellationToken cancellationToken)
    {
        IEnumerable<string> deviceTokens = notification.Devices.Select(x => x.Token);

        AddToGroupRequest addToClientsGroupRequest = new(
            deviceTokens,
            Constants.Groups.ClientsNotificationGroup);
        await _pushNotificationSender.AddToGroup(addToClientsGroupRequest);
    }
}
