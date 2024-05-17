using Swashbuckle.AspNetCore.Filters;

using Tor.Application.StaffMembers.Commands.UpdateWeeklySchedule;

namespace Tor.Api.Examples;

public class UpdateStaffMemberWeeklyScheduleCommandExample : IExamplesProvider<UpdateStaffMemberWeeklyScheduleCommand>
{
    public UpdateStaffMemberWeeklyScheduleCommand GetExamples()
    {
        return new(Guid.NewGuid(), CommonExamples.WeeklySchedule);
    }
}
