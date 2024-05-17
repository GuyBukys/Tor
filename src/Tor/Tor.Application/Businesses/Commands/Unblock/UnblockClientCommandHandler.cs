using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.Businesses.Commands.Unblock;

internal sealed class UnblockClientCommandHandler : IRequestHandler<UnblockClientCommand, Result>
{
    private readonly ITorDbContext _context;

    public UnblockClientCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UnblockClientCommand request, CancellationToken cancellationToken)
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

        await _context.BlockedClients
            .Where(x => x.BusinessId == request.BusinessId)
            .Where(x => x.ClientId == request.ClientId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Ok();
    }
}
