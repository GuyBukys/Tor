using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.Service;

public class GetDefaultServicesResponse
{
    public List<DefaultServiceResponse> DefaultServices { get; set; } = [];
}

public class DefaultServiceResponse
{
    public string Name { get; set; } = string.Empty;
    public AmountDetails Amount { get; set; } = default!;
    public List<Duration> Durations { get; set; } = default!;
}