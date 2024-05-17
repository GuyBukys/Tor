using Bogus;
using Tor.Application.Clients.Commands.Create;
using Tor.Application.Clients.Notifications.ClientCreated;
using Tor.Application.Clients.Queries.GetAppointments;
using Tor.Domain.ClientAggregate;
using Tor.Domain.ClientAggregate.Entities;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class Clients
    {
        // Domain
        public static readonly Faker<Client> ClientFaker = new Faker<Client>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.CreatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.BirthDate, _ => DateOnly.FromDateTime(DateTime.UtcNow))
            .RuleFor(x => x.UpdatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.PhoneNumber, _ => Common.PhoneNumberFaker.Generate())
            .RuleFor(x => x.User, _ => Users.UserFaker.Generate())
            .RuleFor(x => x.UserId, (_, client) => client.User!.Id);

        public static readonly Faker<FavoriteBusiness> FavoriteBusinessFaker = new Faker<FavoriteBusiness>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.ClientId, _ => Guid.NewGuid())
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.MuteNotifications, _ => false);

        // Commands
        public static readonly Faker<GetClientAppointmentsQuery> GetClientAppointmentsQueryFaker = new RecordFaker<GetClientAppointmentsQuery>()
            .RuleFor(x => x.ClientId, _ => Guid.NewGuid());

        public static readonly Faker<CreateClientCommand> CreateClientCommandFaker = new RecordFaker<CreateClientCommand>()
            .RuleFor(x => x.UserId, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Address, _ => Common.AddressFaker.Generate())
            .RuleFor(x => x.PhoneNumber, _ => Common.PhoneNumberFaker.Generate())
            .RuleFor(x => x.ProfileImage, _ => Images.ImageFaker.Generate())
            .RuleFor(x => x.BirthDate, _ => DateOnly.FromDateTime(DateTime.UtcNow));

        // Notifications 
        public static readonly Faker<ClientCreatedNotification> ClientCreatedNotificationFaker = new RecordFaker<ClientCreatedNotification>()
            .RuleFor(x => x.ClientId, _ => Guid.NewGuid())
            .RuleFor(x => x.Devices, f => f.Make(2, () => new Device(Guid.NewGuid().ToString())));
    }
}
