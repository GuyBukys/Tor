using Swashbuckle.AspNetCore.Filters;
using Tor.Application.Users.Commands.AddOrUpdateDevice;

namespace Tor.Api.Examples;

public class AddOrUpdateDeviceCommandExample : IExamplesProvider<AddOrUpdateDeviceCommand>
{
    public AddOrUpdateDeviceCommand GetExamples()
    {
        return new AddOrUpdateDeviceCommand(
            "user token from firebase",
            "device token from FCM");
    }
}
