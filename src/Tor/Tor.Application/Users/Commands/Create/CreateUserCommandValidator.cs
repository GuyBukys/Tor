using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Users.Commands.Create;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserToken)
            .NotEmpty();

        RuleFor(x => x.DeviceToken)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .SetValidator(new PhoneNumberValidator());

        RuleFor(x => x.AppType)
            .IsInEnum();
    }
}
