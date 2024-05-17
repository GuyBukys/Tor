using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Queries.GetAllClients;

internal sealed class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, Result<GetAllClientsResult>>
{
    private readonly ITorDbContext _context;

    public GetAllClientsQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetAllClientsResult>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        Business? business = await _context.Businesses
            .AsNoTracking()
            .Include(x => x.Clients)
            .Include(x => x.BlockedClients)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (business is null)
        {
            return Result.Fail<GetAllClientsResult>(
                new NotFoundError($"could not find business with id {request.Id}"));
        }

        List<ClientResult> clientsWhoBookedAnAppointment = business.Clients
            .Select(x => new ClientResult(x.Id, x.Name, x.PhoneNumber))
            .ToList();

        List<ClientResult> blockedClients = business.BlockedClients
            .Select(x => new ClientResult(x.Id, x.Name, x.PhoneNumber))
            .ToList();

        List<ClientResult> favoriteClients = await _context.FavoriteBusinesses
            .AsNoTracking()
            .Where(x => x.BusinessId == business.Id)
            .Select(x => new ClientResult(x.Client.Id, x.Client.Name, x.Client.PhoneNumber))
            .ToListAsync(cancellationToken);

        return new GetAllClientsResult(
            clientsWhoBookedAnAppointment,
            favoriteClients,
            blockedClients);
    }
}
