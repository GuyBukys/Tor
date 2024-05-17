using Domain;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.Common.ValueObjects;
using Tor.Domain.UserAggregate;
using Tor.Domain.WaitingListAggregate;

namespace Tor.Domain.ClientAggregate;

public sealed class Client : Entity<Guid>
{
    public Client(Guid id) : base(id) { }

    public bool IsActive { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public PhoneNumber PhoneNumber { get; set; } = default!;
    public Address? Address { get; set; } = null!;
    public Image? ProfileImage { get; set; } = null!;

    public List<Appointment> Appointments { get; set; } = [];
    public List<Business> Businesses { get; set; } = [];
    public List<WaitingList> WaitingLists { get; set; } = [];
    public User User { get; set; } = default!;

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public Client()
        : base(Guid.Empty)
    {

    }
    #endregion
}
