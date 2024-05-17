using Tor.Domain.Common.ValueObjects;

namespace Tor.Domain.BusinessAggregate.ValueObjects;

public class DailySchedule
{
    public bool IsWorkingDay { get; set; }
    public TimeRange? TimeRange { get; set; } = null!;
    public List<RecurringBreak> RecurringBreaks { get; set; } = new();

    public DailySchedule(bool isWorkingDay, TimeRange? timeRange, List<RecurringBreak> recurringBreaks)
    {
        IsWorkingDay = isWorkingDay;
        TimeRange = timeRange;
        RecurringBreaks = recurringBreaks;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not DailySchedule otherDailySchedule)
        {
            return false;
        }

        return
            IsWorkingDay == otherDailySchedule.IsWorkingDay &&
            TimeRange == otherDailySchedule.TimeRange &&
            RecurringBreaks.SequenceEqual(otherDailySchedule.RecurringBreaks);
    }

    public override int GetHashCode()
    {
        int timeRangeHashCode = TimeRange?.GetHashCode() ?? 0;
        return IsWorkingDay.GetHashCode() + timeRangeHashCode + RecurringBreaks.Sum(x => x.GetHashCode());
    }

    public DailySchedule() { }
}