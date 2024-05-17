using Newtonsoft.Json;

namespace Tor.Infrastructure.Maps;

internal class AutocompleteResponse
{
    [JsonProperty("predictions")]
    public List<Prediction> Predictions { get; set; } = [];
}

internal class Prediction
{
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}
