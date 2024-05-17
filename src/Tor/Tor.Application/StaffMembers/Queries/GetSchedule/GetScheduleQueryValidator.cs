using FluentValidation;

namespace Tor.Application.StaffMembers.Queries.GetSchedule;

public sealed class GetScheduleQueryValidator : AbstractValidator<GetScheduleQuery>
{
    public GetScheduleQueryValidator()
    {
        RuleFor(x => x.From)
            .Must((obj, from) => from! < obj.Until)
            .When(x => x.From.HasValue && x.Until.HasValue)
            .WithMessage("from date must be smaller than until date");
    }
}
