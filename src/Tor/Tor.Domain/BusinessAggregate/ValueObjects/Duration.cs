using Tor.Domain.BusinessAggregate.Enums;

namespace Tor.Domain.BusinessAggregate.ValueObjects;

public record Duration(
    short Order,
    int ValueInMinutes,
    DurationType Type);
