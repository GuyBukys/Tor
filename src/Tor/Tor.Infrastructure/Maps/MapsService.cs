using Geolocation;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tor.Application.Abstractions;

namespace Tor.Infrastructure.Maps;

internal sealed class MapsService : IMapsService
{
    private readonly HttpClient _httpClient;
    private readonly MapsSettings _settings;

    public MapsService(IHttpClientFactory httpClientFactory, IOptions<MapsSettings> settings)
    {
        _httpClient = httpClientFactory.CreateClient(MapsConstants.MapsApiClient);
        _settings = settings.Value;
    }

    public async Task<Coordinate?> GetByAddress(string address, CancellationToken cancellationToken)
    {
        string requestUri = $"geocode/json?address={address}&key={_settings.ApiKey}&language=iw";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var geocodeResponse = JsonConvert.DeserializeObject<GeocodeResponse>(
            await response.Content.ReadAsStringAsync(cancellationToken))!;

        var location = geocodeResponse.Results.First().Geometry.Location;

        return new Coordinate(location.Latitude, location.Longitude);
    }

    public async Task<List<string>> AutocompleteAddress(string input, CancellationToken cancellationToken)
    {
        string requestUri = $"place/autocomplete/json?input={input}&key={_settings.ApiKey}&language=iw";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var autocompleteResponse = JsonConvert.DeserializeObject<AutocompleteResponse>(
            await response.Content.ReadAsStringAsync(cancellationToken));

        return autocompleteResponse is null ? [] :
            autocompleteResponse.Predictions.ConvertAll(x => x.Description);
    }
}
