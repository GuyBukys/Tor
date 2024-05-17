using FluentValidation;

namespace Tor.Application.Businesses.Queries.GetAppointmentsByClient;

public sealed class GetAppointmentsByClientQueryValidator : AbstractValidator<GetAppointmentsByClientQuery>
{
    public GetAppointmentsByClientQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();

        RuleFor(x => x.ClientId)
            .NotEmpty();
    }
}
