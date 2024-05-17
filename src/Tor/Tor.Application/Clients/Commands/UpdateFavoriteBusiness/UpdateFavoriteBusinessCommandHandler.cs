using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.ClientAggregate.Entities;

namespace Tor.Application.Clients.Commands.UpdateFavoriteBusiness;

internal sealed class UpdateFavoriteBusinessCommandHandler : IRequestHandler<UpdateFavoriteBusinessCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateFavoriteBusinessCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateFavoriteBusinessCommand request, CancellationToken cancellationToken)
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

        if (request.IsFavorite)
        {
            await AddOrUpdate(request, cancellationToken);
            return Result.Ok();
        }

        await _context.FavoriteBusinesses
            .Where(x => x.BusinessId == request.BusinessId)
            .Where(x => x.ClientId == request.ClientId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Ok();
    }

    private async Task AddOrUpdate(UpdateFavoriteBusinessCommand request, CancellationToken cancellationToken)
    {
        FavoriteBusiness? favoriteBusiness = await _context.FavoriteBusinesses
            .Where(x => x.BusinessId == request.BusinessId)
            .Where(x => x.ClientId == request.ClientId)
            .FirstOrDefaultAsync(cancellationToken);

        if (favoriteBusiness is not null)
        {
            favoriteBusiness.MuteNotifications = request.MuteNotifications;
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        await _context.FavoriteBusinesses.AddAsync(new FavoriteBusiness(Guid.NewGuid())
        {
            ClientId = request.ClientId,
            BusinessId = request.BusinessId,
            MuteNotifications = request.MuteNotifications,
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
