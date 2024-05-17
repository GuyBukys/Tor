using Swashbuckle.AspNetCore.Filters;
using Tor.Application.Common.Models;
using Tor.Contracts.Business;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Api.Examples;

public class GetAllBusinessesResponseExample : IExamplesProvider<GetAllBusinessesResponse>
{
    public GetAllBusinessesResponse GetExamples()
    {
        return new GetAllBusinessesResponse
        {
            Businesses = new PagedList<BusinessSummaryResponse>(
                [
                    new BusinessSummaryResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "business name",
                        Description = "business description",
                        Logo = CommonExamples.Image,
                        Cover = CommonExamples.Image,
                        IsOpenNow = true,
                        IsFavorite = true,
                        Address = new Address("ashkelon", "hamra", 12, 100, 100, "right way"),
                        PhoneNumber = new PhoneNumber("+972", "522420554"),
                    }
                ],
                1,
                10,
                1000)
        };
    }
}
