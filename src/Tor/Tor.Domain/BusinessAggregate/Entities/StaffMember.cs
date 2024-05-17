using Domain;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;
using Tor.Domain.UserAggregate;
using Tor.Domain.WaitingListAggregate;

namespace Tor.Domain.BusinessAggregate.Entities;

public sealed class StaffMember : Entity<Guid>
{
    public StaffMember(Guid id) : base(id) { }

    public Guid BusinessId { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public PositionType Position { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public PhoneNumber PhoneNumber { get; set; } = default!;
    public Address? Address { get; set; } = null!;
    public Image? ProfileImage { get; set; } = null!;
    public List<CustomWorkingDay> CustomWorkingDays { get; set; } = [];
    public StaffMemberSettings Settings { get; set; } = default!;
    public WeeklySchedule WeeklySchedule { get; set; } = default!;

    public User User { get; set; } = default!;
    public Business Business { get; set; } = default!;
    public List<Service> Services { get; set; } = [];
    public List<Appointment> Appointments { get; set; } = [];
    public List<WaitingList> WaitingLists { get; set; } = [];
    public List<ReservedTimeSlot> ReservedTimeSlots { get; set; } = [];

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public StaffMember()
        : base(Guid.Empty)
    {

    }
    #endregion
}
