using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Clients.Commands.UpdateProfileImage;

public sealed class UpdateClientProfileImageCommandValidator : AbstractValidator<UpdateClientProfileImageCommand>
{
    public UpdateClientProfileImageCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty();

        RuleFor(x => x.ProfileImage)
            .SetValidator(new ImageValidator())
            .When(x => x.ProfileImage is not null);
    }
}
