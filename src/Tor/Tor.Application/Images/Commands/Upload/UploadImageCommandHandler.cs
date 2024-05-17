using FluentResults;
using MediatR;
using Tor.Application.Abstractions;

namespace Tor.Application.Images.Commands.Upload;

internal sealed class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Result<UploadImageResult>>
{
    private readonly IStorageManager _storageManager;

    public UploadImageCommandHandler(IStorageManager storageManager)
    {
        _storageManager = storageManager;
    }

    public async Task<Result<UploadImageResult>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        string fileName = $"{request.EntityType}/{request.ImageType}/{Guid.NewGuid()}";

        Uri newFileUrl = await _storageManager.UploadFile(fileName, "image/jpeg", request.ContentAsBase64, cancellationToken);
        Uri signedUrl = await _storageManager.SignUrl(fileName, cancellationToken);

        return new UploadImageResult(fileName, newFileUrl, signedUrl);
    }
}
