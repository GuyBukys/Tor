using FluentValidation;
using Tor.Domain.AppointmentAggregate.ValueObjects;

namespace Tor.Application.Common.Validators;

public sealed class ServiceDetailsValidator : AbstractValidator<ServiceDetails>
{
    public ServiceDetailsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Amount)
            .NotEmpty()
            .SetValidator(new AmountDetailsValidator());

        RuleFor(x => x.Durations)
            .NotEmpty()
            .ForEach(x => x.NotEmpty())
            .ForEach(x => x.SetValidator(new DurationValidator()));
    }
}
