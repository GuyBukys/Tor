using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Commands.Block;

public record BlockClientCommand(
    Guid BusinessId,
    Guid ClientId) : IRequest<Result>;
