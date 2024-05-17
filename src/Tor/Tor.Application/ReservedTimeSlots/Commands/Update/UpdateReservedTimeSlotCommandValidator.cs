using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.ReservedTimeSlots.Commands.Update;

public sealed class UpdateReservedTimeSlotCommandValidator : AbstractValidator<UpdateReservedTimeSlotCommand>
{
    public UpdateReservedTimeSlotCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.AtDate)
            .NotEmpty();

        RuleFor(x => x.TimeRange)
            .NotEmpty()
            .SetValidator(new TimeRangeValidator());
    }
}
