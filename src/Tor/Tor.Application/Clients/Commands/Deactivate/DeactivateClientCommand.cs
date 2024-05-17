using FluentResults;
using MediatR;

namespace Tor.Application.Clients.Commands.Deactivate;

public record DeactivateClientCommand(Guid ClientId) : IRequest<Result>;
