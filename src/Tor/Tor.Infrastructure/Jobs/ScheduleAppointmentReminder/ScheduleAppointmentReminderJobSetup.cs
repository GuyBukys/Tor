using Microsoft.Extensions.Options;
using Quartz;

namespace Tor.Infrastructure.Jobs.ScheduleAppointmentReminder;

internal sealed class ScheduleAppointmentReminderJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(ScheduleAppointmentReminderJob), Constants.JobsGroup);

        options.AddJob<ScheduleAppointmentReminderJob>(x => x.WithIdentity(jobKey))
            .AddTrigger(t => t
                .WithIdentity(nameof(ScheduleAppointmentReminderJob))
                .ForJob(jobKey)
                .WithCronSchedule("0 0 18 * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"))));
    }
}
