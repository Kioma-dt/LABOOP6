using System.Text.Json.Serialization;
using Forecast.Models.Weather;

namespace Forecast.Shared.Requstes
{
    public class MultipleCitiesRequest
    {
        [JsonPropertyName("cities")]
        public List<City> Cities { get; set; } = new();
    }
}
