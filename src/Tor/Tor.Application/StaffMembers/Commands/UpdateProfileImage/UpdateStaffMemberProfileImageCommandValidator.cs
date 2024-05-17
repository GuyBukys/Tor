using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.StaffMembers.Commands.UpdateProfileImage;

public sealed class UpdateStaffMemberProfileImageCommandValidator : AbstractValidator<UpdateStaffMemberProfileImageCommand>
{
    public UpdateStaffMemberProfileImageCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.ProfileImage)
            .SetValidator(new ImageValidator())
            .When(x => x.ProfileImage is not null);
    }
}
