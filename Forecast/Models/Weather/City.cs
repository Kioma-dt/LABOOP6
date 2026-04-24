using System.Text.Json.Serialization;

namespace Forecast.Models.Weather
{
    public class City
    {
        [JsonPropertyName("city")]
        public string CityName { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }
    }
}
