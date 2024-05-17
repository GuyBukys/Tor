using Swashbuckle.AspNetCore.Filters;
using Tor.Application.Clients.Commands.Create;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Api.Examples;

public class CreateClientCommandExample : IExamplesProvider<CreateClientCommand>
{
    public CreateClientCommand GetExamples()
    {
        return new CreateClientCommand(
            Guid.NewGuid(),
            "client name",
            "email@email.com",
            DateOnly.FromDateTime(DateTime.UtcNow),
            new PhoneNumber("+972", "522429554"),
            new Address("ashkelon", "hamra", 12, 100, 100, "that way"),
            CommonExamples.Image);
    }
}
