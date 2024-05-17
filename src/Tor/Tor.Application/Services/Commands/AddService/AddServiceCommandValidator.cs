using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Services.Commands.AddService;

public sealed class AddServiceCommandValidator : AbstractValidator<AddServiceCommand>
{
    public AddServiceCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
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
