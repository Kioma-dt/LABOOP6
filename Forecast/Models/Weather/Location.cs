using System.Text.Json.Serialization;

namespace Forecast.Models.Weather
{

    public class Location
    {
        [JsonPropertyName("lat")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("lon")]
        public decimal Longitude { get; set; }
    }
}
