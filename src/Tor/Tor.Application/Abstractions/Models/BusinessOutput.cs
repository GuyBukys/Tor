using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Abstractions.Models;

public record BusinessOutput(
    Guid Id,
    string Name,
    string Description,
    Image Logo,
    Image Cover,
    Address Address,
    PhoneNumber PhoneNumber,
    WeeklySchedule? WeeklySchedule);
