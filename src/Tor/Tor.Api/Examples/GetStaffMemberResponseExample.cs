using Swashbuckle.AspNetCore.Filters;
using Tor.Contracts.Service;
using Tor.Contracts.StaffMember;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Api.Examples;

public class GetStaffMemberResponseExample : IExamplesProvider<GetStaffMemberResponse>
{
    public GetStaffMemberResponse GetExamples()
    {
        return new GetStaffMemberResponse
        {
            Id = Guid.NewGuid(),
            Name = "test staff member",
            Email = "test@test.com",
            Description = "this is a staff member description",
            Address = new("Ashkelon", "Hamra 12", 34, 100, 100, "next exit to the right"),
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow),
            PhoneNumber = new("+972", "522420554"),
            Position = PositionType.BusinessOwner,
            ProfileImage = CommonExamples.Image,
            Services =
                        [
                            new ServiceResponse
                            {
                                Id = Guid.NewGuid(),
                                Name = "test service",
                                Description = "this is a test service",
                                Amount = new(100, "ILS"),
                                Durations =
                                [
                                    new Duration(1, 60, DurationType.Work),
                                    new Duration(2, 10, DurationType.Gap),
                                    new Duration(3, 30, DurationType.Work),
                                ],
                            },
                        ],
            WeeklySchedule = CommonExamples.WeeklySchedule,
            Settings = new(true, true),
        };
    }
}
