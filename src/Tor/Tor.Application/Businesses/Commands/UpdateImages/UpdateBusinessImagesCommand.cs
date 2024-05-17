using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Businesses.Commands.UpdateImages;

public record UpdateBusinessImagesCommand(
    Guid BusinessId,
    Image? Logo,
    Image? Cover,
    List<Image>? Portfolio) : IRequest<Result>;
