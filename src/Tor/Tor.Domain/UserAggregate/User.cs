using Domain;
using Tor.Domain.Common.ValueObjects;
using Tor.Domain.UserAggregate.Enum;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Domain.UserAggregate;

public sealed class User : Entity<Guid>
{
    public User(Guid id) : base(id)
    {
    }

    public bool IsActive { get; set; }
    public string UserToken { get; set; } = string.Empty;
    public bool FirstLogin { get; set; }
    public AppType AppType { get; set; }
    public Guid? EntityId { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public PhoneNumber PhoneNumber { get; set; } = default!;
    public List<Device> Devices { get; set; } = [];

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public User()
        : base(Guid.Empty)
    {

    }
    #endregion
}
