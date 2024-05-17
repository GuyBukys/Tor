using Tor.Domain.Common.ValueObjects;

namespace Tor.Domain.BusinessAggregate.ValueObjects;

public record RecurringBreak
{
    public short Interval { get; set; }
    public TimeRange TimeRange { get; set; } = default!;

    public RecurringBreak() { }

    public RecurringBreak(short interval, TimeRange timeRange)
    {
        Interval = interval;
        TimeRange = timeRange;
    }
}