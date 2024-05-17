using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Services.Queries.GetDefaultServices;

public record DefaultService(
    string Name,
    AmountDetails Amount,
    List<Duration> Durations);
