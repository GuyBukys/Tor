using Mapster;
using Tor.Application.Businesses.Queries.GetAllBusinesses;
using Tor.Application.Businesses.Queries.GetAllClients;
using Tor.Application.Businesses.Queries.GetByReferralCode;
using Tor.Application.Common.Models;
using Tor.Contracts.Business;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate;

namespace Tor.Api.Mappings;

public class BusinessMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BusinessSummary, BusinessSummaryResponse>();
        config.NewConfig<PagedList<BusinessSummary>, GetAllBusinessesResponse>()
            .ConstructUsing(x => new GetAllBusinessesResponse
            {
                Businesses = new PagedList<BusinessSummaryResponse>(
                    x.Items.Adapt<List<BusinessSummaryResponse>>(),
                    x.Page,
                    x.PageSize,
                    x.TotalCount),
            });

        config.NewConfig<Business, GetBusinessResponse>();

        config.NewConfig<ClientResult, GetClientResponse>();
        config.NewConfig<GetAllClientsResult, GetAllClientsResponse>()
            .Map(dest => dest.ClientsWhoBookedAnAppointment, src => src.ClientsWhoBookedAnAppointment)
            .Map(dest => dest.ClientsWhoMarkedAsFavorite, src => src.ClientsWhoMarkedAsFavorite)
            .Map(dest => dest.BlockedClients, src => src.BlockedClients);

        config.NewConfig<GetByReferralCodeResult, GetByReferralCodeResponse>();

        config.NewConfig<Appointment, AppointmentByClientResponse>()
            .Map(dest => dest.StaffMemberName, src => src.StaffMember.Name);
        config.NewConfig<List<Appointment>, GetAppointmentsByClientResponse>()
            .Map(dest => dest.Appointments, src => src);
    }
}
