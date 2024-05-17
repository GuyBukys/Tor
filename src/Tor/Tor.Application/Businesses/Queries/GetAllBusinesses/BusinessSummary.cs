using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Businesses.Queries.GetAllBusinesses;

public record BusinessSummary(
    Guid Id,
    string Name,
    string Description,
    Image Logo,
    Image Cover,
    Address Address,
    PhoneNumber PhoneNumber,
    bool IsOpenNow,
    bool IsFavorite);
