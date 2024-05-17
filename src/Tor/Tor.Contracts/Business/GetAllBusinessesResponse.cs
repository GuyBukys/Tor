using Tor.Application.Common.Models;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.Business;

public class GetAllBusinessesResponse
{
    public PagedList<BusinessSummaryResponse> Businesses { get; set; } = default!;
}

public class BusinessSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Image Logo { get; set; } = default!;
    public Image Cover { get; set; } = default!;
    public Address Address { get; set; } = default!;
    public PhoneNumber PhoneNumber { get; set; } = default!;
    public bool IsOpenNow { get; set; }
    public bool IsFavorite { get; set; }
}
