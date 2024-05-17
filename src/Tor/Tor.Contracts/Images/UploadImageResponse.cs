namespace Tor.Contracts.Images;

public class UploadImageResponse
{
    public string NewFileName { get; set; } = string.Empty;
    public Uri OriginalUrl { get; set; } = default!;
    public Uri SignedUrl { get; set; } = default!;
}
