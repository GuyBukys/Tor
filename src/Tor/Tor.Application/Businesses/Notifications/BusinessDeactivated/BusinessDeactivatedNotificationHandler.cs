using MediatR;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;

namespace Tor.Application.Businesses.Notifications.BusinessDeactivated;

internal sealed class BusinessDeactivatedNotificationHandler : INotificationHandler<BusinessDeactivatedNotification>
{
    private readonly IPushNotificationSender _pushNotificationSender;

    public BusinessDeactivatedNotificationHandler(IPushNotificationSender pushNotificationSender)
    {
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(BusinessDeactivatedNotification notification, CancellationToken cancellationToken)
    {
        await RemoveFromGroup(notification.DeviceTokens);
    }

    private async Task RemoveFromGroup(List<string> deviceTokens)
    {
        RemoveFromGroupRequest request = new(
            deviceTokens,
            Constants.Groups.BusinessesNotificationGroup);

        await _pushNotificationSender.RemoveFromGroup(request);
    }
}
