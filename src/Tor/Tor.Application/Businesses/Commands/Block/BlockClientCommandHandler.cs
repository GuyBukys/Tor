using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.Businesses.Commands.Block;

internal sealed class BlockClientCommandHandler : IRequestHandler<BlockClientCommand, Result>
{
    private readonly ITorDbContext _context;

    public BlockClientCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(BlockClientCommand request, CancellationToken cancellationToken)
    {
        bool isBusinessExists = await _context.Businesses
            .AnyAsync(x => x.Id == request.BusinessId, cancellationToken);
        if (!isBusinessExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        bool isClientExists = await _context.Clients
            .AnyAsync(x => x.Id == request.ClientId, cancellationToken);
        if (!isClientExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        await AddIfNotExists(request, cancellationToken);

        return Result.Ok();
    }

    private async Task AddIfNotExists(BlockClientCommand request, CancellationToken cancellationToken)
    {
        bool isExists = await _context.BlockedClients
            .AnyAsync(x => x.BusinessId == request.BusinessId && x.ClientId == request.ClientId, cancellationToken);
        if (isExists)
        {
            return;
        }

        await _context.BlockedClients.AddAsync(new BlockedClient
        {
            BusinessId = request.BusinessId,
            ClientId = request.ClientId,
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
