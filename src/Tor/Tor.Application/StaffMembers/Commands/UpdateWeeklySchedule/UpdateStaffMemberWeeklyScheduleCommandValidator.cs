using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.StaffMembers.Commands.UpdateWeeklySchedule;

public sealed class UpdateStaffMemberWeeklyScheduleCommandValidator : AbstractValidator<UpdateStaffMemberWeeklyScheduleCommand>
{
    public UpdateStaffMemberWeeklyScheduleCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.WeeklySchedule)
            .NotEmpty()
            .SetValidator(new WeeklyScheduleValidator());
    }
}
