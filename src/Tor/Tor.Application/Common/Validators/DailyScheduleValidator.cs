using FluentValidation;
using Tor.Application.Common.Extensions;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Common.Validators;

public sealed class DailyScheduleValidator : AbstractValidator<DailySchedule>
{
    public DailyScheduleValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.TimeRange)
            .NotNull()
            .SetValidator(new TimeRangeValidator())
            .When(x => x.IsWorkingDay);

        RuleFor(x => x.RecurringBreaks)
            .NotNull()
            .ForEach(x => x.NotEmpty())
            .ForEach(x => x.SetValidator(new RecurringBreakValidator()))
            .When(x => x.IsWorkingDay);

        RuleFor(x => x)
            .Must(x => x.IsRecurringBreaksInsideTimeRange())
            .When(x => x.IsWorkingDay && x.RecurringBreaks is not null)
            .WithMessage("daily schedule: some recurring breaks are not in daily schedule time range");
    }
}
