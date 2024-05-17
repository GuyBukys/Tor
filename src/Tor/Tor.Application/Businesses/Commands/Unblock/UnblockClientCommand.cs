using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Commands.Unblock;

public record UnblockClientCommand(
    Guid BusinessId,
    Guid ClientId) : IRequest<Result>;
