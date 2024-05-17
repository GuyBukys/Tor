using FluentValidation;
using Tor.Application.Common.Validators;

namespace Tor.Application.Appointments.Commands.ScheduleAppointment;

public sealed class ScheduleAppointmentCommandValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.StaffMemberId)
            .NotEmpty();

        RuleFor(x => x.Type)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.ScheduledFor)
            .NotEmpty();

        RuleFor(x => x.ClientDetails)
            .NotEmpty()
            .SetValidator(new ClientDetailsValidator());

        RuleFor(x => x.ServiceDetails)
            .NotEmpty()
            .SetValidator(new ServiceDetailsValidator());
    }
}
