namespace Tor.Domain.Common.ValueObjects;

public record Address(
    string City,
    string Street,
    short? ApartmentNumber,
    double? Latitude,
    double? Longtitude,
    string? Instructions);