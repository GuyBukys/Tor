using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Common;
using Tor.Application.Common.Extensions;
using Tor.Domain.ClientAggregate;
using Tor.Domain.Common.Enums;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Application.Appointments.Notifications.AppointmentCanceled;

internal sealed class AppointmentCanceledNotificationHandler : INotificationHandler<AppointmentCanceledNotification>
{
    private readonly ITorDbContext _context;
    private readonly IPushNotificationSender _pushNotificationSender;

    public AppointmentCanceledNotificationHandler(ITorDbContext context, IPushNotificationSender pushNotificationSender)
    {
        _context = context;
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(AppointmentCanceledNotification notification, CancellationToken cancellationToken)
    {
        string businessName = await _context.Businesses
            .Where(x => x.Id == notification.BusinessId)
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);

        if (notification.NotifyStaffMember)
        {
            await NotifyStaffMember(
                notification.StaffMemberId,
                notification.ClientName,
                notification.AppointmentScheduledDate,
                cancellationToken);
        }

        if (notification.NotifyClient && notification.ClientId is not null)
        {
            await NotifyClient(
                notification.ClientId.Value,
                businessName,
                notification.ServiceName,
                notification.AppointmentScheduledDate,
                notification.Reason,
                cancellationToken);
        }

        if (notification.NotifyWaitingList)
        {
            await NotifyWaitingList(notification.StaffMemberId, businessName, notification.ServiceName, notification.AppointmentScheduledDate, cancellationToken);
        }
    }

    private async Task NotifyStaffMember(
        Guid staffMemberId,
        string clientName,
        DateTimeOffset appointmentDate,
        CancellationToken cancellationToken)
    {
        List<Device>? devices = await _context.Users
            .AsNoTracking()
            .Where(x => x.EntityId == staffMemberId)
            .Select(x => x.Devices)
            .FirstOrDefaultAsync(cancellationToken);

        if (devices is null || devices.Count == 0)
        {
            return;
        }

        string message = AppointmentMessageBuilder.BuildStaffMemberAppointmentCanceledMessage(appointmentDate, clientName);
        Dictionary<string, string> metadata = new()
        {
            { "EntityType", EntityType.StaffMember.ToString() },
            { "ScheduledFor", $"{appointmentDate:s}Z" },
            { "NotificationType", "CancelAppointment" },
        };

        SendPushNotificationRequest pushRequest = new(
            devices.Select(x => x.Token),
            "התבטל תור",
            message,
            metadata);

        await _pushNotificationSender.Send(pushRequest, cancellationToken);
    }

    private async Task NotifyClient(
        Guid clientId,
        string businessName,
        string serviceName,
        DateTimeOffset appointmentDate,
        string? reason,
        CancellationToken cancellationToken)
    {
        List<Device>? devices = await _context.Users
            .AsNoTracking()
            .Where(x => x.EntityId == clientId)
            .Select(x => x.Devices)
            .FirstOrDefaultAsync(cancellationToken);

        if (devices is null || devices.Count == 0)
        {
            return;
        }

        string message = AppointmentMessageBuilder.BuildClientAppointmentCanceledMessage(appointmentDate, serviceName, reason);
        Dictionary<string, string> metadata = new()
        {
            { "EntityType", EntityType.Client.ToString() },
            { "ScheduledFor", $"{appointmentDate:s}Z" },
            { "NotificationType", "CancelAppointment" },
        };

        SendPushNotificationRequest pushRequest = new(
            devices.Select(x => x.Token),
            businessName,
            message,
            metadata);

        await _pushNotificationSender.Send(pushRequest, cancellationToken);
    }

    private async Task NotifyWaitingList(
        Guid staffMemberId,
        string businessName,
        string serviceName,
        DateTimeOffset appointmentDate,
        CancellationToken cancellationToken)
    {
        DateOnly date = DateOnly.FromDateTime(appointmentDate.ToIsraelTime());

        List<Client>? clients = await _context.WaitingLists
            .AsNoTracking()
            .Where(x => x.StaffMemberId == staffMemberId)
            .Where(x => x.AtDate == date)
            .Select(x => x.Clients)
            .FirstOrDefaultAsync(cancellationToken);

        if (clients is null || clients.Count == 0)
        {
            return;
        }

        string message = AppointmentMessageBuilder.BuildWaitingListMessage(serviceName, date);
        Dictionary<string, string> metadata = new()
        {
            { "EntityType", EntityType.Client.ToString() },
            { "ScheduledFor", $"{appointmentDate:s}Z" },
            { "NotificationType", "CancelAppointment" },
        };

        IEnumerable<Guid> clientIds = clients.Select(x => x.Id);
        List<Device> devices = await _context.Users
            .AsNoTracking()
            .Where(x => x.EntityId != null)
            .Where(x => clientIds.Contains(x.EntityId!.Value))
            .SelectMany(x => x.Devices)
            .ToListAsync(cancellationToken);

        SendPushNotificationRequest pushRequest = new(
            devices.Select(x => x.Token),
            businessName,
            message);

        await _pushNotificationSender.Send(pushRequest, cancellationToken);
    }
}
