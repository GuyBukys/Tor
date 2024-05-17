namespace Tor.Application.Abstractions.Models;

public record RefreshTokenInput(string Token, Guid UserId);
