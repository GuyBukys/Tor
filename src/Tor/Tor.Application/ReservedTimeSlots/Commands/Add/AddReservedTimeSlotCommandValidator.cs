using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.ReservedTimeSlots.Commands.Add;

public sealed class AddReservedTimeSlotCommandValidator : AbstractValidator<AddReservedTimeSlotCommand>
{
    public AddReservedTimeSlotCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.AtDate)
            .NotEmpty();

        RuleFor(x => x.TimeRange)
            .NotEmpty()
            .SetValidator(new TimeRangeValidator());
    }
}
