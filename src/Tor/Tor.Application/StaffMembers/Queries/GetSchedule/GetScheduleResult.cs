using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.StaffMembers.Queries.GetSchedule;

public record GetScheduleResult(
    List<Appointment> Appointments,
    List<ReservedTimeSlot> ReservedTimeSlots);
