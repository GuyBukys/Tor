using MediatR;

namespace Tor.Application.Clients.Notifications.ClientDeactivated;

public record ClientDeactivatedNotification(List<string> DeviceTokens) : INotification;
