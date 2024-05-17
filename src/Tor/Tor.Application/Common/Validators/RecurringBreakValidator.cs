using FluentValidation;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Common.Validators;

public sealed class RecurringBreakValidator : InlineValidator<RecurringBreak>
{
    public RecurringBreakValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.Interval)
            .Must(x => x >= 0)
            .WithMessage("interval must be a positive number");

        RuleFor(x => x.TimeRange)
            .NotNull()
            .SetValidator(new TimeRangeValidator());
    }
}
