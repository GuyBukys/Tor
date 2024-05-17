using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Businesses.Commands.UpdateAddress;

public sealed class UpdateBusinessAddressCommandValidator : AbstractValidator<UpdateBusinessAddressCommand>
{
    public UpdateBusinessAddressCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();

        RuleFor(x => x.Address)
            .NotEmpty()
            .SetValidator(new AddressValidator());
    }
}
