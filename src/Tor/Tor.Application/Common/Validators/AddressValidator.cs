using FluentValidation;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Common.Validators;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty();

        RuleFor(x => x.Street)
            .NotEmpty();
    }
}
