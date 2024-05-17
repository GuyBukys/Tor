using Tor.Contracts.Category;
using Tor.Domain.CategoryAggregate;
using Mapster;

namespace Tor.Api.Mappings;

public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryResponse>();
        config.NewConfig<IEnumerable<Category>, GetCategoriesResponse>()
            .Map(dest => dest.Categories, src => src);
    }
}
