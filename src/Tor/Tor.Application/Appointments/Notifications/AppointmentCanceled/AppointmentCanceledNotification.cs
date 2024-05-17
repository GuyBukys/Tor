using MediatR;

namespace Tor.Application.Appointments.Notifications.AppointmentCanceled;

public record AppointmentCanceledNotification(
    Guid BusinessId,
    Guid StaffMemberId,
    string ServiceName,
    Guid? ClientId,
    string ClientName,
    DateTimeOffset AppointmentScheduledDate,
    bool NotifyWaitingList,
    bool NotifyClient,
    bool NotifyStaffMember,
    string? Reason) : INotification;
