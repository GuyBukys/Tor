using FluentValidation;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Common.Validators;

public sealed class CustomWorkingDayValidator : AbstractValidator<CustomWorkingDay>
{
    public CustomWorkingDayValidator()
    {
        RuleFor(x => x.AtDate)
            .NotEmpty();

        RuleFor(x => x.DailySchedule)
            .NotEmpty()
            .SetValidator(new DailyScheduleValidator());
    }
}
