using FluentValidation;

namespace Tor.Application.Businesses.Queries.GetById;

public sealed class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
{
    public GetByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
