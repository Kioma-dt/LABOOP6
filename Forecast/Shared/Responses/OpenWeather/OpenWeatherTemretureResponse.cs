using System.Text.Json.Serialization;

namespace Forecast.Shared.Responses.OpenWeather
{
    public class OpenWeatherTemretureResponse
    {
        [JsonPropertyName("main")]
        public required Nested Main { get; set; }

        public class Nested
        {
            [JsonPropertyName("temp")]
            public decimal Temp { get; set; }
        }
    }

}
