using Domain;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.AppointmentAggregate.ValueObjects;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;

namespace Tor.Domain.AppointmentAggregate;

public class Appointment : Entity<Guid>
{
    public Appointment(Guid id) : base(id)
    {
    }

    public Guid StaffMemberId { get; set; }
    public Guid? ServiceId { get; set; }
    public Guid? ClientId { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public DateTimeOffset ScheduledFor { get; set; }
    public AppointmentType Type { get; set; }
    public AppointmentStatusType Status { get; set; }
    public ClientDetails ClientDetails { get; set; } = default!;
    public ServiceDetails ServiceDetails { get; set; } = default!;
    public string? Notes { get; set; } = null!;
    public bool HasReceivedReminder { get; set; }

    public StaffMember StaffMember { get; set; } = default!;
    public Service? Service { get; set; } = null!;
    public Client? Client { get; set; } = null!;

    #region  parameterless ctor
    /// <summary>
    /// DO NOT USE! FOR TESTING ONLY!
    /// </summary>
    public Appointment()
        : base(Guid.Empty)
    {

    }
    #endregion

    public override string ToString()
    {
        return $"Id: {Id}. Scheduled For: {ScheduledFor}. Status: {Status}. Service details: {ServiceDetails}. Client details: {ClientDetails}";
    }
}
