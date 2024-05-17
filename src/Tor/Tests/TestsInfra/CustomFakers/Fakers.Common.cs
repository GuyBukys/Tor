using Bogus;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class Common
    {
        public static readonly Faker<PhoneNumber> PhoneNumberFaker = new RecordFaker<PhoneNumber>()
            .RuleFor(x => x.Prefix, _ => "+972")
            .RuleFor(x => x.Number, _ => "522420554");

        public static readonly Faker<SocialMedia> SocialMediaFaker = new RecordFaker<SocialMedia>()
            .RuleFor(x => x.Type, f => f.PickRandom<SocialMediaType>())
            .RuleFor(x => x.Url, f => f.Internet.Url());

        public static readonly Faker<AmountDetails> AmountDetailsFaker = new RecordFaker<AmountDetails>()
            .RuleFor(x => x.Amount, f => f.Finance.Amount())
            .RuleFor(x => x.Currency, _ => "ILS");

        public static readonly Faker<Address> AddressFaker = new RecordFaker<Address>()
            .RuleFor(x => x.Street, f => f.Address.StreetAddress())
            .RuleFor(x => x.City, f => f.Address.City())
            .RuleFor(x => x.ApartmentNumber, f => f.Random.Short(1, 100))
            .RuleFor(x => x.Longtitude, f => f.Address.Longitude())
            .RuleFor(x => x.Latitude, f => f.Address.Latitude())
            .RuleFor(x => x.Instructions, f => f.Lorem.Sentence());

        public static readonly Faker<TimeRange> TimeRangeFaker = new RecordFaker<TimeRange>()
            .RuleFor(x => x.StartTime, f => new TimeOnly(f.Random.Int(1, 12), 0))
            .RuleFor(x => x.EndTime, f => new TimeOnly(f.Random.Int(13, 23), 0));

        public static readonly Faker<RecurringBreak> RecurringBreakFaker = new RecordFaker<RecurringBreak>()
            .RuleFor(x => x.Interval, f => f.Random.Short(1, 1000))
            .RuleFor(x => x.TimeRange, _ => TimeRangeFaker.Generate());

        public static readonly Faker<DailySchedule> DailyScheduleFaker = new RecordFaker<DailySchedule>()
            .RuleFor(x => x.IsWorkingDay, _ => true)
            .RuleFor(x => x.TimeRange, _ => TimeRangeFaker.Generate())
            .RuleFor(x => x.RecurringBreaks, (f, dailySchedule) => f.Make(1, () => RecurringBreakFaker.Generate() with
            {
                TimeRange = new(dailySchedule.TimeRange!.StartTime.AddMinutes(1), dailySchedule.TimeRange!.EndTime.AddMinutes(-1)),
            }));

        public static readonly Faker<WeeklySchedule> WeeklyScheduleFaker = new RecordFaker<WeeklySchedule>()
            .RuleFor(x => x.Sunday, _ => DailyScheduleFaker.Generate())
            .RuleFor(x => x.Monday, _ => DailyScheduleFaker.Generate())
            .RuleFor(x => x.Tuesday, _ => DailyScheduleFaker.Generate())
            .RuleFor(x => x.Wednesday, _ => DailyScheduleFaker.Generate())
            .RuleFor(x => x.Thursday, _ => DailyScheduleFaker.Generate())
            .RuleFor(x => x.Friday, _ => DailyScheduleFaker.Generate())
            .RuleFor(x => x.Saturday, _ => DailyScheduleFaker.Generate());
    }
}
