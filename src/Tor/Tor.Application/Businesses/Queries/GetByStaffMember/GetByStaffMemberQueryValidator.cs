using FluentValidation;

namespace Tor.Application.Businesses.Queries.GetByStaffMember;

public sealed class GetByStaffMemberQueryValidator : AbstractValidator<GetByStaffMemberQuery>
{
    public GetByStaffMemberQueryValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();
    }
}
