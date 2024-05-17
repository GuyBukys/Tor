using Swashbuckle.AspNetCore.Filters;
using Tor.Application.Users.Commands.Create;
using Tor.Domain.Common.ValueObjects;
using Tor.Domain.UserAggregate.Enum;

namespace Tor.Api.Examples;

public class CreateUserCommandExample : IExamplesProvider<CreateUserCommand>
{
    public CreateUserCommand GetExamples()
    {
        return new CreateUserCommand(
            "user token from Firebase",
            "device token from FCM",
            new PhoneNumber("+972", "522420554"),
            AppType.BusinessApp);
    }
}
