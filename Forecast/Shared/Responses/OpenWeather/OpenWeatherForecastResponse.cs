using System.Text.Json.Serialization;

namespace Forecast.Shared.Responses.OpenWeather
{
    public class OpenWeatherForecastResponse
    {
        [JsonPropertyName("list")]
        public List<OpenWeatherForecastList> List { get; set; }
    }

    public class OpenWeatherForecastList
    {
        [JsonPropertyName("dt")]
        public int Date { get; set; }

        [JsonPropertyName("temp")]
        public OpenWeatherForecastTempreture Temperature { get; set; }

        [JsonPropertyName("humidity")]
        public decimal Humidity { get; set; }

        [JsonPropertyName("speed")]
        public decimal Speed { get; set; }
    }

    public class OpenWeatherForecastTempreture
    {
        [JsonPropertyName("min")]
        public decimal Min { get; set; }

        [JsonPropertyName("max")]
        public decimal Max { get; set; }
    }
}
