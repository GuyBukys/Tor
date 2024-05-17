using FluentValidation;

namespace Tor.Application.Tiers.Commands.UpdateTier;

public sealed class UpdateTierCommandValidator : AbstractValidator<UpdateTierCommand>
{
    public UpdateTierCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();

        RuleFor(x => x.TierType)
            .IsInEnum();
    }
}
