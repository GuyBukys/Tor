using Swashbuckle.AspNetCore.Filters;

using Tor.Application.StaffMembers.Commands.AddStaffMember;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Api.Examples;

public class AddStaffMemberCommandExample : IExamplesProvider<AddStaffMemberCommand>
{
    public AddStaffMemberCommand GetExamples()
    {
        return new AddStaffMemberCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test@test.com",
            "Guy Buky",
            CommonExamples.WeeklySchedule,
            new("+972", "522420554"),
            new List<AddStaffMemberServiceCommand>()
            {
                new AddStaffMemberServiceCommand(
                    "test service",
                    new(100, "ILS"),
                    new List<Duration>() { new(1, 60, DurationType.Work) }),
            },
            new Image("profile image", new Uri("https://google.com/")));
    }
}
