using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Common;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.Common.Enums;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Application.Appointments.Notifications.AppointmentScheduled;

internal sealed class AppointmentScheduledNotificationHandler : INotificationHandler<AppointmentScheduledNotification>
{
    private readonly ITorDbContext _context;
    private readonly IPushNotificationSender _pushNotificationSender;

    public AppointmentScheduledNotificationHandler(
        ITorDbContext context,
        IPushNotificationSender pushNotificationSender)
    {
        _context = context;
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(AppointmentScheduledNotification notification, CancellationToken cancellationToken)
    {
        if (notification.ClientId is null)
        {
            return;
        }

        if (notification.NotifyStaffMember)
        {
            await NotifyStaffMember(
                notification.StaffMemberId,
                notification.ClientName,
                notification.ScheduledFor,
                cancellationToken);
        }

        await AddClientIfNotExists(notification.BusinessId, notification.ClientId.Value, cancellationToken);

        if (!notification.NotifyClient)
        {
            return;
        }

        var businessDetails = await _context.Businesses
            .Where(x => x.Id == notification.BusinessId)
            .Select(x => new { x.Name, x.NotesForAppointment })
            .FirstAsync(cancellationToken);

        await NotifyClient(
            businessDetails.Name,
            notification.ClientId.Value,
            notification.ScheduledFor,
            businessDetails.NotesForAppointment,
            cancellationToken);

    }

    private async Task NotifyStaffMember(
        Guid staffMemberId,
        string clientName,
        DateTimeOffset scheduledFor,
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

        string message = AppointmentMessageBuilder.BuildStaffMemberAppointmentScheduledMessage(scheduledFor, clientName);
        Dictionary<string, string> metadata = new()
        {
            { "EntityType", EntityType.StaffMember.ToString() },
            { "ScheduledFor", $"{scheduledFor:s}Z" },
            { "NotificationType", "ScheduleAppointment" },
        };

        SendPushNotificationRequest pushRequest = new(
            devices.Select(x => x.Token),
            "נקבע תור",
            message,
            metadata);

        await _pushNotificationSender.Send(pushRequest, cancellationToken);
    }

    private async Task AddClientIfNotExists(Guid businessId, Guid clientId, CancellationToken cancellationToken)
    {
        bool isExists = await _context.BusinessClients
            .Where(x => x.BusinessId == businessId)
            .Where(x => x.ClientId == clientId)
            .AnyAsync(cancellationToken);

        if (isExists)
        {
            return;
        }

        await _context.BusinessClients.AddAsync(new BusinessClient
        {
            BusinessId = businessId,
            ClientId = clientId,
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task NotifyClient(
        string businessName,
        Guid clientId,
        DateTimeOffset scheduledFor,
        string? notesForAppointment,
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

        string message = AppointmentMessageBuilder.BuildClientAppointmentScheduledMessage(businessName, scheduledFor, notesForAppointment);
        Dictionary<string, string> metadata = new()
        {
            { "EntityType", EntityType.Client.ToString() },
            { "ScheduledFor", $"{scheduledFor:s}Z" },
            { "NotificationType", "ScheduleAppointment" },
        };

        SendPushNotificationRequest request = new(
            devices.Select(x => x.Token),
            businessName,
            message,
            metadata);

        await _pushNotificationSender.Send(request, cancellationToken);
    }
}
