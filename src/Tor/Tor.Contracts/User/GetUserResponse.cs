using Tor.Domain.Common.ValueObjects;
using Tor.Domain.UserAggregate.Enum;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Contracts.User;

public class GetUserResponse
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public string UserToken { get; set; } = string.Empty;
    public bool FirstLogin { get; set; }
    public AppType AppType { get; set; }
    public Guid? EntityId { get; set; }
    public PhoneNumber PhoneNumber { get; set; } = default!;
    public List<Device> Devices { get; set; } = new();
}
