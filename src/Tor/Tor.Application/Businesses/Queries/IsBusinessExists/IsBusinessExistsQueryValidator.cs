using FluentValidation;

namespace Tor.Application.Businesses.Queries.IsBusinessExists;

public sealed class IsBusinessExistsQueryValidator : AbstractValidator<IsBusinessExistsQuery>
{
    public IsBusinessExistsQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
