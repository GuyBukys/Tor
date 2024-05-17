using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Businesses.Commands.UpdateImages;

public sealed class UpdateBusinessImagesCommandValidator : AbstractValidator<UpdateBusinessImagesCommand>
{
    public UpdateBusinessImagesCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();

        RuleFor(x => x.Logo)
            .SetValidator(new ImageValidator())
            .When(x => x.Logo is not null);

        RuleFor(x => x.Cover)
            .SetValidator(new ImageValidator())
            .When(x => x.Logo is not null);

        RuleFor(x => x.Portfolio)
            .ForEach(x => x.NotNull())
            .ForEach(x => x.SetValidator(new ImageValidator()))
            .When(x => x.Portfolio is not null);
    }
}
