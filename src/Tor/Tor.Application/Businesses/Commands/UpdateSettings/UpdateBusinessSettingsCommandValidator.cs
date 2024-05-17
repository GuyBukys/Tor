using FluentValidation;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.Businesses.Commands.UpdateSettings;

public sealed class UpdateBusinessSettingsCommandValidator : AbstractValidator<UpdateBusinessSettingsCommand>
{
    public UpdateBusinessSettingsCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();

        RuleFor(x => x.BusinessSettings)
            .NotNull()
            .SetValidator(new BusinessSettingsValidator());
    }
}

internal sealed class BusinessSettingsValidator : InlineValidator<BusinessSettings>
{
    internal BusinessSettingsValidator()
    {
        RuleFor(x => x.BookingMinimumTimeInAdvanceInMinutes)
            .GreaterThan(0);

        RuleFor(x => x.BookingMaximumTimeInAdvanceInDays)
            .GreaterThan(0)
            .Must((x, value) => x.BookingMinimumTimeInAdvanceInMinutes < (value * 1440))
            .WithMessage("business settings: booking minimum time in advance cannot be greater than booking maximum time in advance");

        RuleFor(x => x.CancelAppointmentMinimumTimeInMinutes)
            .GreaterThan(0);

        RuleFor(x => x.RescheduleAppointmentMinimumTimeInMinutes)
            .GreaterThan(0);

        RuleFor(x => x.MaximumAppointmentsForClient)
            .GreaterThan(0);

        RuleFor(x => x.AppointmentReminderTimeInHours)
            .GreaterThan(0);
    }
}