using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Appointments.Queries.GetAvailableTimes;

public record AvailableTimes(DateOnly Date, List<TimeRange> TimeRanges);
