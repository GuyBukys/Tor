namespace Tor.Contracts.StaffMember;

public class GetScheduleResponse
{
    public List<StaffMemberAppointmentResponse> Appointments { get; set; } = [];
    public List<ReservedTimeSlotResponse> ReservedTimeSlots { get; set; } = [];
}
