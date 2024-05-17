namespace Tor.Application.Images.Commands.Upload;

public record UploadImageResult(
    string FileName,
    Uri OriginalUrl,
    Uri SignedUrl);
