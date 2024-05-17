using Swashbuckle.AspNetCore.Filters;
using Tor.Contracts.Category;
using Tor.Domain.CategoryAggregate.Enums;

namespace Tor.Api.Examples;

public class GetCategoriesResponseExample : IExamplesProvider<GetCategoriesResponse>
{
    public GetCategoriesResponse GetExamples()
    {
        return new GetCategoriesResponse
        {
            Categories = new List<CategoryResponse>()
            {
                new CategoryResponse
                {
                    DisplayName = "מספרה",
                    Type = CategoryType.Barbershop,
                    Url = "https://google.com",
                },
            }
        };
    }
}
