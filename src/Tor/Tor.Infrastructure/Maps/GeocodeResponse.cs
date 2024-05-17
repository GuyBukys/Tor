using Newtonsoft.Json;

namespace Tor.Infrastructure.Maps;

internal class GeocodeResponse
{
    public List<Geocode> Results { get; set; } = [];
}

internal class Geocode
{
    [JsonProperty("formatted_address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("geometry")]
    public Geometry Geometry { get; set; } = default!;
}

internal class Geometry
{
    [JsonProperty("location")]
    public Location Location { get; set; } = default!;
}

internal class Location
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }

    [JsonProperty("lng")]
    public double Longitude { get; set; }
}