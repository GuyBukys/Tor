using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.UserAggregate;

namespace Tor.Application.MessageBlasts.Commands.ScheduleAppointmentReminder;

internal sealed class ScheduleAppointmentReminderCommandHandler : IRequestHandler<ScheduleAppointmentReminderCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IPushNotificationSender _pushNotificationSender;

    public ScheduleAppointmentReminderCommandHandler(ITorDbContext context, IPushNotificationSender pushNotificationSender)
    {
        _context = context;
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task<Result> Handle(ScheduleAppointmentReminderCommand request, CancellationToken cancellationToken)
    {
        var businessMessageBlasts = await _context.BusinessMessageBlasts
            .AsNoTracking()
            .Include(x => x.MessageBlast)
            .Where(x => x.MessageBlast.Name == Constants.MessageBlasts.ScheduleAppointmentReminderName)
            .Where(x => x.IsActive)
            .Select(x => new
            {
                x.Body,
                x.MessageBlast.TemplateBody,
                StaffMemberIds = x.Business.StaffMembers.Select(x => x.Id),
                BusinessName = x.Business.Name,
            })
            .ToListAsync(cancellationToken);

        foreach (var businessMessageBlast in businessMessageBlasts)
        {
            List<User> usersWithLastAppointmentOneMonthAgo = await _context.Appointments
                .Where(x => x.ClientId != null)
                .Where(x => businessMessageBlast.StaffMemberIds.Contains(x.StaffMemberId))
                .Where(x => x.Status != AppointmentStatusType.Canceled)
                .Where(x => DateTime.UtcNow.Date - x.ScheduledFor.Date == TimeSpan.FromDays(30))
                .Select(x => x.Client!.User)
                .ToListAsync(cancellationToken);

            if (usersWithLastAppointmentOneMonthAgo.Count == 0)
            {
                continue;
            }

            IEnumerable<string> deviceTokens = usersWithLastAppointmentOneMonthAgo
                .SelectMany(x => x.Devices)
                .Select(x => x.Token);

            SendPushNotificationRequest pushRequest = new(
                deviceTokens,
                businessMessageBlast.BusinessName,
                businessMessageBlast.Body ?? businessMessageBlast.TemplateBody);

            await _pushNotificationSender.Send(pushRequest, cancellationToken);
        }

        return Result.Ok();
    }
}
