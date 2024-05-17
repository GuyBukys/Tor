using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Contracts.StaffMember;

public class GetCustomWorkingDaysResponse
{
    public List<CustomWorkingDay> CustomWorkingDays { get; set; } = new();
}
