using Swashbuckle.AspNetCore.Filters;

using Tor.Contracts.StaffMember;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Api.Examples;

public class GetCustomWorkingDaysResponseExample : IExamplesProvider<GetCustomWorkingDaysResponse>
{
    public GetCustomWorkingDaysResponse GetExamples()
    {
        return new GetCustomWorkingDaysResponse
        {
            CustomWorkingDays =
            [
                new CustomWorkingDay(DateOnly.FromDateTime(DateTime.UtcNow), CommonExamples.WeeklySchedule.Sunday),
            ],
        };
    }
}
