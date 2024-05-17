using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Commands.Deactivate;

public record DeactivateBusinessCommand(Guid BusinessId) : IRequest<Result>;
