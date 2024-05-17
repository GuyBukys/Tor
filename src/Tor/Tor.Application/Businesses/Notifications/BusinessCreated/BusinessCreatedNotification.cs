using MediatR;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Application.Businesses.Notifications.BusinessCreated;

public record BusinessCreatedNotification(
    Guid BusinessId,
    string BusinessName,
    IEnumerable<Device> Devices) : INotification;
