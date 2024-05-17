using Bogus;
using Tor.Application.ReservedTimeSlots.Commands.Add;
using Tor.Application.ReservedTimeSlots.Commands.Update;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.Common.ValueObjects;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class ReservedTimeSlots
    {
        // Domain
        public static readonly Faker<ReservedTimeSlot> ReservedTimeSlotFaker = new Faker<ReservedTimeSlot>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.AtDate, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
            .RuleFor(x => x.Reason, f => f.Lorem.Sentence())
            .RuleFor(x => x.TimeRange, _ => new TimeRange(new TimeOnly(Random.Shared.Next(1, 12), 0), new TimeOnly(Random.Shared.Next(13, 23), 0)))
            .RuleFor(x => x.CreatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedDateTime, _ => DateTime.UtcNow);

        // Commands
        public static readonly Faker<AddReservedTimeSlotCommand> AddReservedTimeSlotCommandFaker = new RecordFaker<AddReservedTimeSlotCommand>()
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.AtDate, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
            .RuleFor(x => x.Reason, f => f.Lorem.Sentence())
            .RuleFor(x => x.TimeRange, _ => new TimeRange(new TimeOnly(Random.Shared.Next(1, 12), 0), new TimeOnly(Random.Shared.Next(13, 23), 0)));

        public static readonly Faker<UpdateReservedTimeSlotCommand> UpdateReservedTimeSlotCommandFaker = new RecordFaker<UpdateReservedTimeSlotCommand>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.AtDate, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
            .RuleFor(x => x.Reason, f => f.Lorem.Sentence())
            .RuleFor(x => x.TimeRange, _ => new TimeRange(new TimeOnly(Random.Shared.Next(1, 12), 0), new TimeOnly(Random.Shared.Next(13, 23), 0)));
    }
}
