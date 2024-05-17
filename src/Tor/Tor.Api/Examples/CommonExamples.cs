using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Api.Examples;

public static class CommonExamples
{
    public static readonly Image Image = new("image name", new Uri("https://google.com/"));

    public static readonly WeeklySchedule WeeklySchedule = new WeeklySchedule(
                new DailySchedule(
                    true,
                    new(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                    new List<RecurringBreak>()
                    {
                        new RecurringBreak(0, new(new TimeOnly(12, 0), new TimeOnly(14, 0))),
                    }),
                new DailySchedule(
                    true,
                    new(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                    new List<RecurringBreak>()
                    {
                        new RecurringBreak(0, new(new TimeOnly(12, 0), new TimeOnly(14, 0))),
                    }),
                new DailySchedule(
                    true,
                    new(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                    new List<RecurringBreak>()
                    {
                        new RecurringBreak(0, new(new TimeOnly(12, 0), new TimeOnly(14, 0))),
                    }),
                new DailySchedule(
                    true,
                    new(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                    new List<RecurringBreak>()
                    {
                        new RecurringBreak(0, new(new TimeOnly(12, 0), new TimeOnly(14, 0))),
                    }),
                new DailySchedule(
                    true,
                    new(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                    new List<RecurringBreak>()
                    {
                        new RecurringBreak(0, new(new TimeOnly(12, 0), new TimeOnly(14, 0))),
                    }),
                new DailySchedule(
                    true,
                    new(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                    new List<RecurringBreak>()
                    {
                        new RecurringBreak(0, new(new TimeOnly(12, 0), new TimeOnly(14, 0))),
                    }),
                new DailySchedule(
                    true,
                    new(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                    new List<RecurringBreak>()
                    {
                        new RecurringBreak(0, new(new TimeOnly(12, 0), new TimeOnly(14, 0))),
                    }));
}
