using System.Text.Json.Serialization;

namespace Forecast.Shared.Responses.GoogleWeather
{

    public class GoogleWeatherForecastResponse
    {
        [JsonPropertyName("forecastDays")]
        public List<GoogleWeatherForecastDay> ForecastDays { get; set; }
    }

    public class GoogleWeatherForecastDay
    {
        [JsonPropertyName("displayDate")]
        public GoogleWeatherDisplayDate DisplayDate { get; set; }

        [JsonPropertyName("daytimeForecast")]
        public GoogleWeatherDaytimeForecast DaytimeForecast { get; set; }

        [JsonPropertyName("maxTemperature")]
        public GoogleWeatherForecastMaxTemperature MaxTemperature { get; set; }

        [JsonPropertyName("minTemperature")]
        public GoogleWeatherForecastMinTemperature MinTemperature { get; set; }
    }

    public class GoogleWeatherDisplayDate
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("month")]
        public int Month { get; set; }

        [JsonPropertyName("day")]
        public int Day { get; set; }
    }

    public class GoogleWeatherDaytimeForecast
    {
        [JsonPropertyName("relativeHumidity")]
        public decimal RelativeHumidity { get; set; }

        [JsonPropertyName("wind")]
        public GoogleWeatherForecastWind Wind { get; set; }
    }

    public class GoogleWeatherForecastWind
    {
        [JsonPropertyName("speed")]
        public GoogleWeatherForecastSpeed Speed { get; set; }
    }

    public class GoogleWeatherForecastSpeed
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }

    public class GoogleWeatherForecastMaxTemperature
    {
        [JsonPropertyName("degrees")]
        public decimal Degrees { get; set; }
    }

    public class GoogleWeatherForecastMinTemperature
    {
        [JsonPropertyName("degrees")]
        public decimal Degrees { get; set; }
    }
}
