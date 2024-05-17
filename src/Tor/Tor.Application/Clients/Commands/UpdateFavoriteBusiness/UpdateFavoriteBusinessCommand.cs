using FluentResults;
using MediatR;

namespace Tor.Application.Clients.Commands.UpdateFavoriteBusiness;

public record UpdateFavoriteBusinessCommand(
    Guid ClientId,
    Guid BusinessId,
    bool IsFavorite,
    bool MuteNotifications) : IRequest<Result>;
