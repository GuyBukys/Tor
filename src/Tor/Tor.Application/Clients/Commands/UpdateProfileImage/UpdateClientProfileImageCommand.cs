using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Clients.Commands.UpdateProfileImage;

public record UpdateClientProfileImageCommand(
    Guid ClientId,
    Image? ProfileImage) : IRequest<Result>;
