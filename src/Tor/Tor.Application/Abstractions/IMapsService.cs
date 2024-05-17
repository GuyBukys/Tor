using Geolocation;

namespace Tor.Application.Abstractions;

public interface IMapsService
{
    Task<Coordinate?> GetByAddress(string address, CancellationToken cancellationToken);
    Task<List<string>> AutocompleteAddress(string input, CancellationToken cancellationToken);
}
