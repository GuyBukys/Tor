namespace Tor.Application.Abstractions.Models;

public class GenerateTokenResult
{
    public required string JwtToken { get; set; }
    public required string RefreshToken { get; set; }
}
