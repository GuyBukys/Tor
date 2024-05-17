using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using Tor.Application.MessageBlasts.Commands.ScheduleAppointmentReminder;

namespace Tor.Infrastructure.Jobs.ScheduleAppointmentReminder;

/// <summary>
/// Sends a notification to all clients of a business
/// who havent booked a new appointment in over a month
/// </summary>
[DisallowConcurrentExecution]
internal sealed class ScheduleAppointmentReminderJob : IJob
{
    private readonly ILogger<ScheduleAppointmentReminderJob> _logger;
    private readonly ISender _sender;

    public ScheduleAppointmentReminderJob(ILogger<ScheduleAppointmentReminderJob> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("started job {jobName}", nameof(ScheduleAppointmentReminderJob));

        var result = await _sender.Send(new ScheduleAppointmentReminderCommand(), context.CancellationToken);

        _logger.LogInformation("finished job {jobName}", nameof(ScheduleAppointmentReminderJob));
    }
}
