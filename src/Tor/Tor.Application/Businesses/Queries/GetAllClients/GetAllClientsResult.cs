using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Businesses.Queries.GetAllClients;

public record GetAllClientsResult(
    List<ClientResult> ClientsWhoBookedAnAppointment,
    List<ClientResult> ClientsWhoMarkedAsFavorite,
    List<ClientResult> BlockedClients);

public record ClientResult(
    Guid Id,
    string Name,
    PhoneNumber PhoneNumber);