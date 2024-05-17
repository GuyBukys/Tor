using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.ClientAggregate;

namespace Tor.Application.Clients.Queries.GetById;

internal sealed class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, Result<Client>>
{
    private readonly ITorDbContext _context;

    public GetClientByIdQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Client>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        Client? client = await _context.Clients
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (client is null)
        {
            return Result.Fail<Client>(
                new NotFoundError($"could not find client with id {request.Id}"));
        }

        return client!;
    }
}
