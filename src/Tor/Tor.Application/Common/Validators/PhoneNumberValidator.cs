using Tor.Domain.Common.ValueObjects;
using FluentValidation;

namespace Tor.Application.Common.Validators;

public class PhoneNumberValidator : AbstractValidator<PhoneNumber>
{
    public PhoneNumberValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Prefix)
            .NotEmpty()
            .Matches("^\\+\\d{1,3}$");

        RuleFor(x => x.Number)
            .NotEmpty()
            .Matches("^\\d+$");
    }
}
