using FluentValidation;

namespace Tor.Application.Tiers.Queries.ValidateTier;

public sealed class ValidateTierQueryValidator : AbstractValidator<ValidateTierQuery>
{
    public ValidateTierQueryValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();
    }
}
