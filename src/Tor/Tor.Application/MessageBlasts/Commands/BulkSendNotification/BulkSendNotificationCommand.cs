using FluentResults;
using MediatR;

namespace Tor.Application.MessageBlasts.Commands.BulkSendNotification;

public record BulkSendNotificationCommand(
    Guid BusinessId,
    string Title,
    string Message) : IRequest<Result>;
