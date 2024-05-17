using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.Common.Enums;

namespace Tor.Application.Images.Commands.Upload;

public record UploadImageCommand(
    string ContentAsBase64,
    ImageType ImageType,
    EntityType EntityType) : IRequest<Result<UploadImageResult>>;
