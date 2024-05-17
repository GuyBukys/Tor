using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Common;
using Tor.Application.Common.Extensions;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.Common.Enums;

namespace Tor.Application.MessageBlasts.Commands.AppointmentReminder;

internal sealed class AppointmentReminderCommandHandler : IRequestHandler<AppointmentReminderCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<AppointmentReminderCommandHandler> _logger;

    public AppointmentReminderCommandHandler(
        ITorDbContext context,
        IPushNotificationSender pushNotificationSender,
        ILogger<AppointmentReminderCommandHandler> logger)
    {
        _context = context;
        _pushNotificationSender = pushNotificationSender;
        _logger = logger;
    }

    public async Task<Result> Handle(AppointmentReminderCommand request, CancellationToken cancellationToken)
    {
        var appointments = (await _context.Appointments
            .Where(x => !x.HasReceivedReminder)
            .Where(x => x.ClientId != null)
            .Where(x => x.Status != AppointmentStatusType.Canceled)
            .Where(x => x.ScheduledFor.UtcDateTime > DateTimeOffset.UtcNow)
            .Select(x => new
            {
                x.Id,
                BusinessName = x.StaffMember.Business.Name,
                x.Client!.User,
                ClientName = x.ClientDetails.Name,
                x.ScheduledFor,
                x.Notes,
                x.StaffMember.Business.Settings.AppointmentReminderTimeInHours,
                ServiceName = x.ServiceDetails.Name,
            })
            .ToListAsync(cancellationToken))
            .Where(x =>
                (x.ScheduledFor - DateTime.UtcNow).TotalHours > 0 &&
                (x.ScheduledFor - DateTime.UtcNow).TotalHours < x.AppointmentReminderTimeInHours)
            .ToList();

        if (appointments.Count == 0)
        {
            _logger.LogInformation("no appointments to send a reminder were found");
            return Result.Ok();
        }

        _logger.LogInformation("found {appointmentCount} to send a reminder. appointments: {@appointments}", appointments.Count, appointments);

        foreach (var appointment in appointments)
        {
            string message = AppointmentMessageBuilder.BuildAppointmentReminderMessage(
                appointment.ClientName,
                appointment.ServiceName,
                appointment.ScheduledFor.ToIsraelTime(),
                appointment.Notes);

            Dictionary<string, string> metadata = new()
            {
                { "EntityType", EntityType.Client.ToString() },
                { "ScheduledFor", $"{appointment.ScheduledFor:s}Z" },
                { "NotificationType", "AppointmentReminder" },
                {"AppointmentId", appointment.Id.ToString() },
            };

            IEnumerable<string> deviceTokens = appointment.User.Devices.Select(x => x.Token);

            SendPushNotificationRequest pushRequest = new(deviceTokens, appointment.BusinessName, message);
            await _pushNotificationSender.Send(pushRequest, cancellationToken);
        }

        var appointmentIds = appointments.Select(x => x.Id);
        await _context.Appointments
            .Where(x => appointmentIds.Contains(x.Id))
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.HasReceivedReminder, true), cancellationToken);

        return Result.Ok();
    }
}
