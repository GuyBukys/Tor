using FluentResults;
using MediatR;

namespace Tor.Application.MessageBlasts.Commands.Activate;

public record ActivateMessageBlastCommand(
    Guid BusinessId,
    Guid MessageBlastId,
    string? Body) : IRequest<Result>;
