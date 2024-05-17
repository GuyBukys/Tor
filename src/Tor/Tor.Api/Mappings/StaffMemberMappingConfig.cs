using Mapster;
using Tor.Application.StaffMembers.Queries.GetSchedule;
using Tor.Contracts.StaffMember;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Api.Mappings;

public class StaffMemberMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<StaffMember, AddStaffMemberResponse>()
            .Map(dest => dest.StaffMemberId, src => src.Id);

        config.NewConfig<StaffMember, GetStaffMemberResponse>();

        config.NewConfig<ReservedTimeSlot, ReservedTimeSlotResponse>();
        config.NewConfig<Appointment, StaffMemberAppointmentResponse>();
        config.NewConfig<GetScheduleResult, GetScheduleResponse>();
    }
}
