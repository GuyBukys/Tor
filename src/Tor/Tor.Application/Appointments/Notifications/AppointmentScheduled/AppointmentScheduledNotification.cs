using MediatR;

namespace Tor.Application.Appointments.Notifications.AppointmentScheduled;

public record AppointmentScheduledNotification(
    Guid? ClientId,
    Guid StaffMemberId,
    string ClientName,
    Guid BusinessId,
    DateTimeOffset ScheduledFor,
    bool NotifyClient,
    bool NotifyStaffMember) : INotification;
