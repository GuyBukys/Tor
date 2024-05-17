using Microsoft.Extensions.Options;
using Quartz;

namespace Tor.Infrastructure.Jobs.AppointmentReminder;

public class AppointmentReminderJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(AppointmentReminderJob), Constants.JobsGroup);

        options.AddJob<AppointmentReminderJob>(x => x.WithIdentity(jobKey))
            .AddTrigger(t => t
                .WithIdentity(nameof(AppointmentReminderJob))
                .ForJob(jobKey)
                .WithSimpleSchedule(s => s
                    .WithIntervalInMinutes(5)
                    .RepeatForever()));
    }
}
