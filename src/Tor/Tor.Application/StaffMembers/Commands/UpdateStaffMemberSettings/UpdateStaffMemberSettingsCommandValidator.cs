using FluentValidation;

namespace Tor.Application.StaffMembers.Commands.UpdateStaffMemberSettings;

public class UpdateStaffMemberSettingsCommandValidator : AbstractValidator<UpdateStaffMemberSettingsCommand>
{
    public UpdateStaffMemberSettingsCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.Settings)
            .NotEmpty();
    }
}
