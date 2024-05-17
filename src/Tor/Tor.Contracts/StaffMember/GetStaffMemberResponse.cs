using Tor.Contracts.Service;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.StaffMember;

public class GetStaffMemberResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public PositionType Position { get; set; }
    public PhoneNumber PhoneNumber { get; set; } = default!;
    public Address? Address { get; set; } = null!;
    public Image? ProfileImage { get; set; } = null!;
    public WeeklySchedule WeeklySchedule { get; set; } = new(
        new DailySchedule(true, new TimeRange(new(8, 0), new(9, 0)), []),
        new DailySchedule(true, new TimeRange(new(8, 0), new(9, 0)), []),
        new DailySchedule(true, new TimeRange(new(8, 0), new(9, 0)), []),
        new DailySchedule(true, new TimeRange(new(8, 0), new(9, 0)), []),
        new DailySchedule(true, new TimeRange(new(8, 0), new(9, 0)), []),
        new DailySchedule(true, new TimeRange(new(8, 0), new(9, 0)), []),
        new DailySchedule(true, new TimeRange(new(8, 0), new(9, 0)), []));
    public List<ServiceResponse> Services { get; set; } = [];
    public StaffMemberSettings Settings { get; set; } = default!;
}
