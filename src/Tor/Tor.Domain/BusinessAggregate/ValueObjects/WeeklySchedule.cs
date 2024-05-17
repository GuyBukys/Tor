namespace Tor.Domain.BusinessAggregate.ValueObjects;

public record WeeklySchedule
{
    public DailySchedule Sunday { get; set; } = default!;
    public DailySchedule Monday { get; set; } = default!;
    public DailySchedule Tuesday { get; set; } = default!;
    public DailySchedule Wednesday { get; set; } = default!;
    public DailySchedule Thursday { get; set; } = default!;
    public DailySchedule Friday { get; set; } = default!;
    public DailySchedule Saturday { get; set; } = default!;

    public WeeklySchedule(
        DailySchedule sunday,
        DailySchedule monday,
        DailySchedule tuesday,
        DailySchedule wednesday,
        DailySchedule thursday,
        DailySchedule friday,
        DailySchedule saturday)
    {
        Sunday = sunday;
        Monday = monday;
        Tuesday = tuesday;
        Wednesday = wednesday;
        Thursday = thursday;
        Friday = friday;
        Saturday = saturday;
    }

    public WeeklySchedule() { }
}
