using FluentResults;
using MediatR;

namespace Tor.Application.Services.Commands.DeleteService;

public record DeleteServiceCommand(Guid ServiceId) : IRequest<Result>;
