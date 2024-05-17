using FluentValidation;
using Tor.Application.Common.Validators;
using Tor.Application.CustomWorkingDays.Commands.AddOrUpdate;

namespace Tor.Application.CustomWorkingDays.Commands.Add;

public sealed class AddCustomWorkingDayCommandValidator : AbstractValidator<AddOrUpdateCustomWorkingDayCommand>
{
    public AddCustomWorkingDayCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.CustomWorkingDay)
            .NotEmpty()
            .SetValidator(new CustomWorkingDayValidator());
    }
}
