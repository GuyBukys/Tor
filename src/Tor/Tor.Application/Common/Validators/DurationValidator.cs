using Tor.Domain.BusinessAggregate.ValueObjects;
using FluentValidation;

namespace Tor.Application.Common.Validators;

public class DurationValidator : AbstractValidator<Duration>
{
    public DurationValidator()
    {
        RuleFor(x => x.ValueInMinutes)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo((short)1);

        RuleFor(x => x.Type)
            .IsInEnum();
    }
}
