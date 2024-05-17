using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Common.Extensions;

internal static class FluentValidatorExtensions
{
    internal static bool IsRecurringBreaksInsideTimeRange(this DailySchedule dailySchedule)
    {
        TimeOnly startTime = dailySchedule.TimeRange!.StartTime;
        TimeOnly endTime = dailySchedule.TimeRange!.EndTime;

        foreach (var recurringBreak in dailySchedule.RecurringBreaks)
        {
            bool isRecurringBreakInsideTimeRange =
                recurringBreak.TimeRange!.StartTime >= startTime &&
                recurringBreak.TimeRange!.EndTime <= endTime;

            if (!isRecurringBreakInsideTimeRange)
            {
                return false;
            }
        }

        return true;
    }
}
