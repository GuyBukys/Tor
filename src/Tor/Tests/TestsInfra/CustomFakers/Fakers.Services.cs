using Bogus;
using Tor.Application.Services.Commands.AddService;
using Tor.Application.Services.Commands.UpdateService;
using Tor.Application.Services.Queries.GetServices;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class Services
    {
        public static readonly Faker<Service> ServiceFaker = new Faker<Service>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.Amount, _ => Common.AmountDetailsFaker.Generate())
            .RuleFor(x => x.CreatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.Location, f => f.PickRandom<LocationType>())
            .RuleFor(x => x.Durations, f => f.Make(1, () => Businesses.DurationFaker.Generate()));

        public static readonly Faker<GetServicesQuery> GetServicesQueryFaker = new RecordFaker<GetServicesQuery>()
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid());

        public static readonly Faker<AddServiceCommand> AddServiceCommandFaker = new RecordFaker<AddServiceCommand>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.StaffMemberId, _ => Guid.NewGuid())
            .RuleFor(x => x.Amount, _ => Common.AmountDetailsFaker.Generate())
            .RuleFor(x => x.Durations, f => f.Make(1, () => Businesses.DurationFaker.Generate()));

        public static readonly Faker<UpdateServiceCommand> UpdateServiceCommandFaker = new RecordFaker<UpdateServiceCommand>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.ServiceId, _ => Guid.NewGuid())
            .RuleFor(x => x.Amount, _ => Common.AmountDetailsFaker.Generate())
            .RuleFor(x => x.Durations, f => f.Make(1, () => Businesses.DurationFaker.Generate()));
    }
}
