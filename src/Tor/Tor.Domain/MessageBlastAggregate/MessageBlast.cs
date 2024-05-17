using Domain;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.MessageBlastAggregate.Enums;

namespace Tor.Domain.MessageBlastAggregate;

public sealed class MessageBlast : Entity<Guid>
{
    public MessageBlast(Guid id) :
        base(id)
    {
        Id = id;
    }

    public bool IsActive { get; set; }
    public MessageBlastType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DisplayDescription { get; set; } = string.Empty;
    public string TemplateBody { get; set; } = string.Empty;
    public bool CanEditBody { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }

    public List<Business> Businesses { get; set; } = [];

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public MessageBlast()
        : base(Guid.Empty)
    {

    }
    #endregion
}
