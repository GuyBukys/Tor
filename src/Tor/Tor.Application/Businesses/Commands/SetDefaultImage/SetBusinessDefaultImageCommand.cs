using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Commands.SetDefaultImage;

public record SetBusinessDefaultImageCommand(
    Guid BusinessId,
    bool SetLogo,
    bool SetCover) : IRequest<Result>;
