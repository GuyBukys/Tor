using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Domain.ClientAggregate;

namespace Tor.Infrastructure.Persistence.Repositories;

internal sealed class ClientRepository : IClientRepository
{
    private readonly TorDbContext _context;

    public ClientRepository(TorDbContext context)
    {
        _context = context;
    }

    public Task<Client> Create(CreateClientInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task Deactivate(Guid clientId, CancellationToken cancellationToken)
    {
        using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        await _context.Clients
            .Where(x => x.Id == clientId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.IsActive, false)
                .SetProperty(p => p.UpdatedDateTime, DateTime.UtcNow),
                cancellationToken);

        await _context.Users
            .Where(x => x.EntityId == clientId)
            .ExecuteDeleteAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
