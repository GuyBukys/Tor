using FluentValidation;

namespace Tor.Application.StaffMembers.Commands.DeleteStaffMember;

public sealed class DeleteStaffMemberCommandValidator : AbstractValidator<DeleteStaffMemberCommand>
{
    public DeleteStaffMemberCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();
    }
}
