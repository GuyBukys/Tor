using FluentValidation;

namespace Tor.Application.Users.Queries.Get;

public sealed class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(x => x.UserToken)
            .NotEmpty();
    }
}
