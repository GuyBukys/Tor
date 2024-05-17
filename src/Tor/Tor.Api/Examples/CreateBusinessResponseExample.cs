using Swashbuckle.AspNetCore.Filters;
using Tor.Contracts.Business;

namespace Tor.Api.Examples;

internal sealed class CreateBusinessResponseExample : IExamplesProvider<CreateBusinessResponse>
{
    public CreateBusinessResponse GetExamples()
    {
        return new CreateBusinessResponse
        {
            Id = Guid.NewGuid(),
            InvitationId = Guid.NewGuid().ToString(),
        };
    }
}
