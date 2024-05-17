using Bogus;
using Tor.Application.CustomWorkingDays.Commands.AddOrUpdate;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class CustomWorkingDays
    {
        public static readonly Faker<CustomWorkingDay> CustomWorkingDayFaker = new RecordFaker<CustomWorkingDay>()
            .RuleFor(x => x.AtDate, _ => DateOnly.FromDateTime(DateTime.UtcNow))
            .RuleFor(x => x.DailySchedule, _ => Common.DailyScheduleFaker.Generate());

        public static readonly Faker<AddOrUpdateCustomWorkingDayCommand> AddOrUpdateCustomWorkingDayCommandFaker = new RecordFaker<AddOrUpdateCustomWorkingDayCommand>()
            .RuleFor(x => x.CustomWorkingDay, _ => CustomWorkingDayFaker.Generate());
    }
}
