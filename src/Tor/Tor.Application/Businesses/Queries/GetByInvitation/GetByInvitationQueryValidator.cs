using FluentValidation;

namespace Tor.Application.Businesses.Queries.GetByInvitation;

public sealed class GetByInvitationQueryValidator : AbstractValidator<GetByInvitationQuery>
{
    public GetByInvitationQueryValidator()
    {
        RuleFor(x => x.InvitationId)
            .NotEmpty();
    }
}
