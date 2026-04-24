using System.Text.Json.Serialization;

namespace Forecast.Shared.Responses.GoogleWeather
{
    public class GoogleWeatherTemretureResponse
    {
        [JsonPropertyName("temperature")]
        public required Nested Tempreature { get; set; }

        public class Nested
        {
            [JsonPropertyName("degrees")]
            public decimal Degrees { get; set; }
        }
    }
}
