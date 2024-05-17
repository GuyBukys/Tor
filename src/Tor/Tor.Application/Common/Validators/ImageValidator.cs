using FluentValidation;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Common.Validators;

public sealed class ImageValidator : AbstractValidator<Image>
{
    public ImageValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("image: not a valid URI!");
    }
}
