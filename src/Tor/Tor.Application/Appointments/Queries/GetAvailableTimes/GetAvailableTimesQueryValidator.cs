using FluentValidation;

namespace Tor.Application.Appointments.Queries.GetAvailableTimes;

public sealed class GetAvailableTimesQueryValidator : AbstractValidator<GetAvailableTimesQuery>
{
    public GetAvailableTimesQueryValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.StartDate < x.EndDate)
            .WithMessage("start date must be smaller than end date");
    }
}
