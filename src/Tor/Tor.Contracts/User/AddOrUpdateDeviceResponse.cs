using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Contracts.User;

public class AddOrUpdateDeviceResponse
{
    public List<Device> Devices { get; set; } = [];
}
