using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Services.Commands.UpdateService;

public sealed class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Amount)
            .NotNull()
            .SetValidator(new AmountDetailsValidator());

        RuleFor(x => x.Durations)
            .NotEmpty()
            .ForEach(x => x.SetValidator(new DurationValidator()))
            .Must(x => x.DistinctBy(x => x.Order).Count() == x.Count());
    }
}
