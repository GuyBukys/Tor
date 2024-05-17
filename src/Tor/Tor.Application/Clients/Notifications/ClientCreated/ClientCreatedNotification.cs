using MediatR;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Application.Clients.Notifications.ClientCreated;

public record ClientCreatedNotification(Guid ClientId, IEnumerable<Device> Devices) : INotification;
