using Swashbuckle.AspNetCore.Filters;

using Tor.Application.Businesses.Commands.UpdateWeeklySchedule;

namespace Tor.Api.Examples;

public class UpdateBusinessWeeklyScheduleCommandExample : IExamplesProvider<UpdateBusinessWeeklyScheduleCommand>
{
    public UpdateBusinessWeeklyScheduleCommand GetExamples()
    {
        return new(Guid.NewGuid(), CommonExamples.WeeklySchedule);
    }
}
