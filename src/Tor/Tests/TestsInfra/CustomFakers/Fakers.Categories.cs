using Bogus;
using Tor.Domain.CategoryAggregate;
using Tor.Domain.CategoryAggregate.Enums;

namespace Tor.TestsInfra.CustomFakers;

public partial class Fakers
{
    public static class Categories
    {
        public static readonly Faker<Category> CategoryFaker = new Faker<Category>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Type, f => f.PickRandom<CategoryType>())
            .RuleFor(x => x.Image, _ => Images.ImageFaker.Generate());
    }
}
