using Tor.Domain.CategoryAggregate.Enums;

namespace Tor.Contracts.Category;

public class GetCategoriesResponse
{
    public IEnumerable<CategoryResponse> Categories { get; set; } = Enumerable.Empty<CategoryResponse>();
}

public class CategoryResponse
{
    public Guid Id { get; set; }
    public CategoryType Type { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
