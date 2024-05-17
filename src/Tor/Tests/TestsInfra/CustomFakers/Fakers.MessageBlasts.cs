using Bogus;
using Tor.Domain.MessageBlastAggregate;
using Tor.Domain.MessageBlastAggregate.Entities;
using Tor.Domain.MessageBlastAggregate.Enums;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class MessageBlasts
    {
        public static readonly Faker<BusinessMessageBlast> BusinessMessageBlastFaker = new Faker<BusinessMessageBlast>()
            .RuleFor(x => x.BusinessId, _ => Guid.NewGuid())
            .RuleFor(x => x.MessageBlastId, _ => Guid.NewGuid())
            .RuleFor(x => x.Body, f => f.Lorem.Sentence())
            .RuleFor(x => x.IsActive, _ => true);

        public static readonly Faker<MessageBlast> MessageBlastFaker = new Faker<MessageBlast>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.TemplateBody, f => f.Lorem.Sentence())
            .RuleFor(x => x.CanEditBody, _ => true)
            .RuleFor(x => x.IsActive, _ => true)
            .RuleFor(x => x.Type, f => f.PickRandom<MessageBlastType>())
            .RuleFor(x => x.CreatedDateTime, _ => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedDateTime, _ => DateTime.UtcNow);
    }
}
