using Bogus;
using Tor.Application.CustomWorkingDays.Queries.GetCustomWorkingDays;
using Tor.Application.StaffMembers.Commands.AddStaffMember;
using Tor.Application.StaffMembers.Commands.UpdateStaffMemberAddress;
using Tor.Application.StaffMembers.Commands.UpdateWeeklySchedule;
using Tor.Application.StaffMembers.Queries.GetSchedule;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class StaffMembers
    {
        // Domain
        public static readonly Faker<StaffMemberSettings> StaffMemberSettingsFaker = new RecordFaker<StaffMemberSettings>();

        public static readonly Faker<Duration> DurationFaker = new RecordFaker<Duration>()
            .RuleFor(x => x.Order, f => (short)(f.IndexFaker + 1))
            .RuleFor(x => x.ValueInMinutes, f => f.Random.Int(10, 60))
            .RuleFor(x => x.Type, f => f.PickRandom<DurationType>());

        public static readonly Faker<StaffMember> StaffMemberFaker = new Faker<StaffMember>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.BirthDate, _ => DateOnly.FromDateTime(DateTime.UtcNow))
            .RuleFor(x => x.PhoneNumber, _ => Common.PhoneNumberFaker.Generate())
            .RuleFor(x => x.Address, _ => Common.AddressFaker.Generate())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.CreatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.Position, f => f.PickRandom<PositionType>())
            .RuleFor(x => x.UserId, _ => Guid.NewGuid())
            .RuleFor(x => x.Services, (f, s) => f.Make(5, () =>
                {
                    var services = Services.ServiceFaker.Generate();
                    services.StaffMemberId = s.Id;
                    return services;
                }))
            .RuleFor(x => x.Settings, _ => StaffMemberSettingsFaker.Generate())
            .RuleFor(x => x.WeeklySchedule, _ => Common.WeeklyScheduleFaker.Generate());

        // Commands
        public static readonly Faker<GetScheduleQuery> GetScheduleQueryFaker = new RecordFaker<GetScheduleQuery>()
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.From, _ => DateTime.UtcNow)
            .RuleFor(x => x.Until, _ => DateTime.UtcNow.AddDays(10));

        public static readonly Faker<UpdateStaffMemberAddressCommand> UpdateStaffMemberAddressCommandFaker = new RecordFaker<UpdateStaffMemberAddressCommand>()
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.Address, _ => Common.AddressFaker.Generate());

        public static readonly Faker<AddStaffMemberServiceCommand> ServiceCommandFaker = new RecordFaker<AddStaffMemberServiceCommand>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Amount, _ => Common.AmountDetailsFaker.Generate())
            .RuleFor(x => x.Durations, f => f.Make(1, () => DurationFaker.Generate()));

        public static readonly Faker<AddStaffMemberCommand> AddStaffMemberCommandFaker = new RecordFaker<AddStaffMemberCommand>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.UserId, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.PhoneNumber, _ => Common.PhoneNumberFaker.Generate())
            .RuleFor(x => x.WeeklySchedule, _ => Common.WeeklyScheduleFaker.Generate())
            .RuleFor(x => x.Services, f => f.Make(2, () => ServiceCommandFaker.Generate()))
            .RuleFor(x => x.ProfileImage, _ => Images.ImageFaker.Generate());

        public static readonly Faker<UpdateStaffMemberWeeklyScheduleCommand> UpdateStaffMemberWeeklyScheduleCommandFaker = new RecordFaker<UpdateStaffMemberWeeklyScheduleCommand>()
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.WeeklySchedule, _ => Common.WeeklyScheduleFaker.Generate());

        // Queries
        public static readonly Faker<GetCustomWorkingDaysQuery> GetCustomWorkingDaysQueryFaker = new RecordFaker<GetCustomWorkingDaysQuery>()
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.From, _ => DateOnly.FromDateTime(DateTime.UtcNow))
            .RuleFor(x => x.Until, _ => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));

    }
}
