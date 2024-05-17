namespace Tor.Domain.BusinessAggregate.ValueObjects;

public record BusinessSettings(
    int BookingMinimumTimeInAdvanceInMinutes,
    int BookingMaximumTimeInAdvanceInDays,
    int CancelAppointmentMinimumTimeInMinutes,
    int RescheduleAppointmentMinimumTimeInMinutes,
    int MaximumAppointmentsForClient,
    int AppointmentReminderTimeInHours);
