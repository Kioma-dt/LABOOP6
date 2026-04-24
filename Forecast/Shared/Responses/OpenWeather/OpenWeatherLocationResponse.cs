using System.Text.Json;
using System.Text.Json.Serialization;

namespace Forecast.Shared.Responses.OpenWeather
{
    //public class OpenWeatherLocationResponse
    //{
    //    public List<OpenWeatherLocationResponseCity> Cities { get; set; } = new();
    //}

    public class OpenWeatherLocationResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lat")]
        public decimal Lat { get; set; }

        [JsonPropertyName("lon")]
        public decimal Lon { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}
