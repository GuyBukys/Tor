using FluentValidation;

namespace Tor.Application.Services.Queries.GetServices;

public sealed class GetServicesQueryValidator : AbstractValidator<GetServicesQuery>
{
    public GetServicesQueryValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();
    }
}
