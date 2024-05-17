using Mapster;
using Tor.Application.Clients.Queries.GetAppointments;
using Tor.Contracts.Client;
using Tor.Domain.ClientAggregate;

namespace Tor.Api.Mappings;

public class ClientMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Client, ClientResponse>();

        config.NewConfig<Client, CreateClientResponse>()
            .Map(dest => dest.ClientId, src => src.Id);

        config.NewConfig<BusinessDetails, BusinessDetailsResponse>();
        config.NewConfig<ClientAppointmentResult, ClientAppointmentResponse>();
        config.NewConfig<List<ClientAppointmentResult>, GetClientAppointmentsResponse>()
            .Map(dest => dest.Appointments, src => src);
    }
}
