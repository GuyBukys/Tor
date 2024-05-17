namespace Tor.Domain.BusinessAggregate.ValueObjects;

public record CustomWorkingDay
{
    public DateOnly AtDate { get; set; }
    public DailySchedule DailySchedule { get; set; } = default!;

    public CustomWorkingDay(DateOnly atDate, DailySchedule dailySchedule)
    {
        AtDate = atDate;
        DailySchedule = dailySchedule;
    }

    public CustomWorkingDay() { }
};