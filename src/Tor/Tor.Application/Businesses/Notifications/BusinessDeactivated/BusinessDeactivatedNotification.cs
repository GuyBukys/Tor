using MediatR;

namespace Tor.Application.Businesses.Notifications.BusinessDeactivated;

public record BusinessDeactivatedNotification(List<string> DeviceTokens) : INotification;
