using FluentResults;
using MediatR;

namespace Tor.Application.MessageBlasts.Commands.Deactivate;

public record DeactivateMessageBlastCommand(
    Guid BusinessId,
    Guid MessageBlastId) : IRequest<Result>;
