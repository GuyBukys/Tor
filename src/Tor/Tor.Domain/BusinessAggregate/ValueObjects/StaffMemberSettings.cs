namespace Tor.Domain.BusinessAggregate.ValueObjects;

public record StaffMemberSettings(
    bool SendNotificationsWhenAppointmentScheduled,
    bool SendNotificationsWhenAppointmentCanceled);
