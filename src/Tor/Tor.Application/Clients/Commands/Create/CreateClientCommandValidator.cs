using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Clients.Commands.Create;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Address)
            .SetValidator(new AddressValidator())
            .When(x => x.Address is not null);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .SetValidator(new PhoneNumberValidator());
    }
}
