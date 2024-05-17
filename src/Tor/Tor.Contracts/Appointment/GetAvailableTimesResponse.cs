using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.Appointment;

public class GetAvailableTimesResponse
{
    public List<AvailableTimesResponse> AvailableTimes { get; set; } = [];
}

public class AvailableTimesResponse
{
    public DateOnly Date { get; set; }
    public List<TimeRange> TimeRanges { get; set; } = [];
}
