using FluentValidation;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Common.Validators;

public class AmountDetailsValidator : AbstractValidator<AmountDetails>
{
    private static readonly HashSet<string> _currencies = new()
    {
        "ILS",
    };

    public AmountDetailsValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(_currencies.Contains)
            .WithMessage("amount details: not a valid currency code. (valid currencies: ILS)");
    }
}
