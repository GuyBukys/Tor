using FluentValidation;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Common.Validators;

public sealed class WeeklyScheduleValidator : InlineValidator<WeeklySchedule>
{
    public WeeklyScheduleValidator()
    {
        RuleFor(x => x.Sunday)
            .NotEmpty()
            .SetValidator(new DailyScheduleValidator());

        RuleFor(x => x.Monday)
            .NotEmpty()
            .SetValidator(new DailyScheduleValidator());

        RuleFor(x => x.Tuesday)
            .NotEmpty()
            .SetValidator(new DailyScheduleValidator());

        RuleFor(x => x.Wednesday)
            .NotEmpty()
            .SetValidator(new DailyScheduleValidator());

        RuleFor(x => x.Thursday)
            .NotEmpty()
            .SetValidator(new DailyScheduleValidator());

        RuleFor(x => x.Friday)
            .NotEmpty()
            .SetValidator(new DailyScheduleValidator());

        RuleFor(x => x.Saturday)
            .NotEmpty()
            .SetValidator(new DailyScheduleValidator());
    }
}
