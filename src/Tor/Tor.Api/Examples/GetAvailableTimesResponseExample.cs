using Swashbuckle.AspNetCore.Filters;
using Tor.Contracts.Appointment;

namespace Tor.Api.Examples;


public class GetAvailableTimesResponseExample : IExamplesProvider<GetAvailableTimesResponse>
{
    public GetAvailableTimesResponse GetExamples()
    {
        return new GetAvailableTimesResponse
        {
            AvailableTimes = [
                new AvailableTimesResponse
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                    TimeRanges = [new(new(8, 0), new(9, 0)), new(new(9, 0), new(10, 0))]
                }
            ]
        };
    }
}
