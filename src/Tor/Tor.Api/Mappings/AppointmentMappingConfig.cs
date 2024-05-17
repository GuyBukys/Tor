using Mapster;
using Tor.Application.Appointments.Queries.GetAvailableTimes;
using Tor.Contracts.Appointment;
using Tor.Domain.AppointmentAggregate;

namespace Tor.Api.Mappings;

public class AppointmentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Appointment, ScheduleAppointmentResponse>()
            .Map(dest => dest.AppointmentId, src => src.Id);

        config.NewConfig<AvailableTimes, AvailableTimesResponse>();
        config.NewConfig<List<AvailableTimes>, GetAvailableTimesResponse>()
            .Map(dest => dest.AvailableTimes, src => src);
    }
}
