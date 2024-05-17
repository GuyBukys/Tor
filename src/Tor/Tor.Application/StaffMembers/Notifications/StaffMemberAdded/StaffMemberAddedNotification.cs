using MediatR;

namespace Tor.Application.StaffMembers.Notifications.StaffMemberAdded;

public record StaffMemberAddedNotification(Guid StaffMemberId) : INotification;
