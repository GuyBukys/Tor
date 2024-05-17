using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using Tor.Application.MessageBlasts.Commands.AppointmentReminder;

namespace Tor.Infrastructure.Jobs.AppointmentReminder;

/// <summary>
/// Sends a reminder to all clients of a business
/// 24 hours prior to an appointment
/// </summary>
[DisallowConcurrentExecution]
public class AppointmentReminderJob : IJob
{
    private readonly ILogger<AppointmentReminderJob> _logger;
    private readonly ISender _sender;

    public AppointmentReminderJob(ILogger<AppointmentReminderJob> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("started job {jobName}", nameof(AppointmentReminderJob));

        var result = await _sender.Send(new AppointmentReminderCommand(), context.CancellationToken);

        _logger.LogInformation("finished job {jobName}", nameof(AppointmentReminderJob));
    }
}
