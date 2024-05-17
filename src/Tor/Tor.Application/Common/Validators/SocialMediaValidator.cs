using Tor.Domain.BusinessAggregate.ValueObjects;
using FluentValidation;

namespace Tor.Application.Common.Validators;

public class SocialMediaValidator : AbstractValidator<SocialMedia>
{
    public SocialMediaValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Url)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out Uri? _))
            .WithMessage("social media: not a valid URL");
    }
}
