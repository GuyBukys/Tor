namespace Tor.Domain.Common.ValueObjects;

public record PhoneNumber(
    string Prefix,
    string Number);
