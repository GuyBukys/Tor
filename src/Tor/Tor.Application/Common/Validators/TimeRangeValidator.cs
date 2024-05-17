using Tor.Domain.Common.ValueObjects;
using FluentValidation;

namespace Tor.Application.Common.Validators;

public sealed class TimeRangeValidator : AbstractValidator<TimeRange>
{
    public TimeRangeValidator()
    {
        RuleFor(x => x)
            .NotNull();

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .Must((timeRange, startTime) => startTime < timeRange.EndTime)
            .WithMessage("TimeRange: start time cannot be greater or equal to end time");
    }
}
