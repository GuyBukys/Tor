using Swashbuckle.AspNetCore.Filters;
using Tor.Contracts.Business;
using Tor.Domain.MessageBlastAggregate.Enums;

namespace Tor.Api.Examples;

public class GetMessageBlastsResponseExample : IExamplesProvider<GetMessageBlastsResponse>
{
    public GetMessageBlastsResponse GetExamples()
    {
        return new GetMessageBlastsResponse
        {
            MessageBlasts =
            [
                new MessageBlastResponse
                {
                    Name = "message blast name",
                    Description = "message blast description",
                    DisplayName = "שם בעברית",
                    DisplayDescription = "תיאור בעברית",
                    TemplateBody = "content of the fixed template",
                    CanEditBody = true,
                    BusinessTitle = "title that the business edited himself",
                    BusinessBody = "content that the business edited himself",
                    IsActive = true,
                    Type = MessageBlastType.FirstImpression,
                }
            ]
        };
    }
}
