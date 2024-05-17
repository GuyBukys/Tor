using FluentValidation;

namespace Tor.Application.Businesses.Queries.GetByReferralCode;

public sealed class GetByReferralCodeQueryValidator : AbstractValidator<GetByReferralCodeQuery>
{
    public GetByReferralCodeQueryValidator()
    {
        RuleFor(x => x.ReferralCode)
            .NotEmpty()
            .MaximumLength(100);
    }
}
