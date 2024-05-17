namespace Tor.Domain.Common.ValueObjects;

public record Image
{
    public string Name { get; set; } = string.Empty;
    public Uri Url { get; set; } = default!;

    public Image(string name, Uri url)
    {
        Name = name;
        Url = url;
    }

    public Image() { }
}
