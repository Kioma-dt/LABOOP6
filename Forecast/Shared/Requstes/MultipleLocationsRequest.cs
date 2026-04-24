using System.Text.Json.Serialization;
using Forecast.Models.Weather;

namespace Forecast.Shared.Requstes
{
    public class MultipleLocationsRequest
    {
        [JsonPropertyName("locations")]
        public List<Location> Locations { get; set; } = new();
    }
}
