using FluentValidation;

namespace Tor.Application.Appointments.Commands.ApproveAppointment;

public sealed class ApproveAppointmentCommandValidator : AbstractValidator<ApproveAppointmentCommand>
{
    public ApproveAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty();
    }
}
