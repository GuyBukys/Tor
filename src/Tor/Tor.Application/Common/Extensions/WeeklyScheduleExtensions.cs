using System.Diagnostics;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Common.Extensions;

internal static class WeeklyScheduleExtensions
{
    internal static DailySchedule GetDailySchedule(this WeeklySchedule weeklySchedule, DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => weeklySchedule.Sunday,
            DayOfWeek.Monday => weeklySchedule.Monday,
            DayOfWeek.Tuesday => weeklySchedule.Tuesday,
            DayOfWeek.Wednesday => weeklySchedule.Wednesday,
            DayOfWeek.Thursday => weeklySchedule.Thursday,
            DayOfWeek.Friday => weeklySchedule.Friday,
            DayOfWeek.Saturday => weeklySchedule.Saturday,
            _ => throw new UnreachableException()
        };
    }
}
