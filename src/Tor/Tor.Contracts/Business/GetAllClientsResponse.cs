using Tor.Domain.Common.ValueObjects;

namespace Tor.Contracts.Business;

public class GetAllClientsResponse
{
    public List<GetClientResponse> ClientsWhoBookedAnAppointment { get; set; } = [];
    public List<GetClientResponse> ClientsWhoMarkedAsFavorite { get; set; } = [];
    public List<GetClientResponse> BlockedClients { get; set; } = [];
}
public class GetClientResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PhoneNumber PhoneNumber { get; set; } = default!;
}
