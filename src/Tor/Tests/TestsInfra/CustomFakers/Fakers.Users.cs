using Bogus;
using Tor.Application.Users.Commands.AddOrUpdateDevice;
using Tor.Application.Users.Commands.Create;
using Tor.Domain.Common.ValueObjects;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class Users
    {
        // Domain
        public static readonly Faker<User> UserFaker = new Faker<User>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.UserToken, _ => Guid.NewGuid().ToString())
            .RuleFor(x => x.Devices, f => f.Make(1, () => new Device(Guid.NewGuid().ToString())))
            .RuleFor(x => x.AppType, f => f.PickRandom<AppType>())
            .RuleFor(x => x.IsActive, f => f.Random.Bool())
            .RuleFor(x => x.FirstLogin, f => f.Random.Bool())
            .RuleFor(x => x.PhoneNumber, f => new PhoneNumber("+972", f.Phone.PhoneNumber()));

        // Commands
        public static readonly Faker<CreateUserCommand> CreateUserCommandFaker = new RecordFaker<CreateUserCommand>()
            .RuleFor(x => x.UserToken, _ => Guid.NewGuid().ToString())
            .RuleFor(x => x.DeviceToken, _ => Guid.NewGuid().ToString())
            .RuleFor(x => x.PhoneNumber, _ => Common.PhoneNumberFaker.Generate())
            .RuleFor(x => x.AppType, f => f.PickRandom<AppType>());

        public static readonly Faker<AddOrUpdateDeviceCommand> AddOrUpdateDeviceCommandFaker = new RecordFaker<AddOrUpdateDeviceCommand>()
            .RuleFor(x => x.UserToken, _ => Guid.NewGuid().ToString())
            .RuleFor(x => x.DeviceToken, _ => Guid.NewGuid().ToString());
    }
}
