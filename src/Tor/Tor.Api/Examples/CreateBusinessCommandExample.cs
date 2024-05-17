using Swashbuckle.AspNetCore.Filters;

using Tor.Application.Businesses.Commands.Create;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Api.Examples;

internal sealed class CreateBusinessCommandExample : IExamplesProvider<CreateBusinessCommand>
{
    public CreateBusinessCommand GetExamples()
    {
        return new CreateBusinessCommand(
            "TestBusiness",
            "this is a test business creation",
            "test@test.com",
            new() { Guid.NewGuid() },
            new BusinessOwnerCommand(
                Guid.NewGuid(),
                "guy buky",
                "businessOwner@test.com",
                new("+972", "522974474"),
                CommonExamples.WeeklySchedule,
                new List<ServiceCommand>()
                {
                    new ServiceCommand(
                        "test service",
                        new(100, "ILS"),
                        new List<Duration>() { new(1, 60, DurationType.Work) }),
                },
                new Image("profile image", new Uri("https://google.com/"))),
            CommonExamples.WeeklySchedule,
            new Address(
                    "Ashkelon",
                    "Hamra 12",
                    34,
                    100,
                    100,
                    "next exit to the right"),
            [new("+972", "522420554")],
            new Image("cover photo", new Uri("https://google.com/")),
            new Image("cover photo", new Uri("https://google.com/")),
            Guid.NewGuid());
    }
}
