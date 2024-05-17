using Tor.Application.Abstractions.Models;
using Tor.Domain.ClientAggregate;

namespace Tor.Application.Abstractions;

public interface IClientRepository
{
    Task<Client> Create(CreateClientInput input, CancellationToken cancellationToken);
    Task Deactivate(Guid clientId, CancellationToken cancellationToken);
}
