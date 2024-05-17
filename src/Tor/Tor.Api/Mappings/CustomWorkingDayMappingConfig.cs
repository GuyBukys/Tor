using Mapster;
using Tor.Contracts.StaffMember;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Api.Mappings;

public class CustomWorkingDayMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<List<CustomWorkingDay>, GetCustomWorkingDaysResponse>()
            .Map(dest => dest.CustomWorkingDays, src => src);
    }
}
