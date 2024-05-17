using FluentValidation;

namespace Tor.Application.Businesses.Commands.Deactivate;

public sealed class DeactivateBusinessCommandValidator : AbstractValidator<DeactivateBusinessCommand>
{
    public DeactivateBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();
    }
}
