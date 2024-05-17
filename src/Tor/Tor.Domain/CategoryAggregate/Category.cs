using Domain;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.CategoryAggregate.Enums;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Domain.CategoryAggregate;

public sealed class Category : Entity<Guid>
{
    public Category(Guid id) : base(id)
    {
    }

    public CategoryType Type { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public Image? Image { get; set; } = null!;

    public List<Business> Businesses { get; set; } = [];

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public Category()
        : base(Guid.Empty)
    {

    }
    #endregion
}
