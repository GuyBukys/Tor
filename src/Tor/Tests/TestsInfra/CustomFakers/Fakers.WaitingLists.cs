using Bogus;
using Tor.Application.WaitingLists.Commands.JoinWaitingList;
using Tor.Domain.WaitingListAggregate;

namespace Tor.TestsInfra.CustomFakers;
public partial class Fakers
{
    public static class WaitingLists
    {
        // Domain
        public static readonly Faker<WaitingList> WaitingListFaker = new Faker<WaitingList>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.AtDate, f => f.Date.FutureDateOnly())
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.Clients, f => f.Make(5, () => Clients.ClientFaker.Generate()));

        // Commands
        public static readonly Faker<JoinWaitingListCommand> JoinWaitingListCommandFaker = new RecordFaker<JoinWaitingListCommand>()
            .RuleFor(x => x.AtDate, f => DateOnly.FromDateTime(DateTime.UtcNow).AddDays(Random.Shared.Next(1, 10000)))
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.ClientId, _ => Guid.NewGuid());
    }
}
