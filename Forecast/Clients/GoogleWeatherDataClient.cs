using Forecast.Utils;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json.Serialization;
using System.Text.Json;
using Forecast.Models.Weather;

namespace Forecast.Clients
{
    public class GoogleWeatherDataClient : IWeatherDataClient
    {
        private readonly HttpClient client;
        private readonly string apiKey;

        public string Provider => "GoogleWeather";

        public GoogleWeatherDataClient(IConfiguration config, HttpClient httpClient) 
        {
            client = httpClient;
            client.BaseAddress = new Uri(config.GetValue<string>("GOOGLEWEATHER_BASE_URL") ?? "");
            apiKey = config.GetValue<string>("GOOGLEWEATHER_API_KEY") ?? "";
        }

        public async Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
        {
            try
            {
                var response = await client.GetAsync(
                    $"/currentConditions:lookup?key={apiKey}&location.latitude={latitude}&location.longitude={longitude}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiCallException(
                        $"googleweather returned bad status: {(ushort)response.StatusCode}"
                    );
                }

                var data = await response.Content.ReadFromJsonAsync<GoogleWeatherResponse>();
                return data?.Tempreature?.Degrees ?? throw new ApiCallException($"failed to decode response");
            }
            catch (JsonException e)
            {
                throw new ApiCallException($"failed to deserialize: {e.Message}.", inner: e);
            }
            catch (HttpRequestException e)
            {
                throw new ApiCallException($"failed to call googleweather: {e.Message}.", inner: e);
            }
        }

        public async Task<DailyForecast> ForecastForDays(decimal latitude, decimal longitude, int days)
        {
            try
            {
                var response = await client.GetAsync(
                    $"/forecast/days:lookup?key={apiKey}&location.latitude={latitude}&location.longitude={longitude}&days={days}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiCallException(
                        $"googleweather returned bad status: {(ushort)response.StatusCode}"
                    );
                }

                var data = await response.Content.ReadFromJsonAsync<GoogleWeatherForecastResponse>();

                if (data is null || data.ForecastDays is null)
                {
                    throw new ApiCallException($"failed to decode response");
                }

                var dailyForecast = new DailyForecast();

                foreach (var item in data.ForecastDays)
                {
                    dailyForecast.DayForecasts.Add(new DayForecast()
                    {
                        Date = new DateTime(item.DisplayDate.Year, item.DisplayDate.Month, item.DisplayDate.Day),
                        MaxTempreture = item.MaxTemperature.Degrees,
                        MinTempreture = item.MinTemperature.Degrees,
                        Humidity = item.DaytimeForecast.RelativeHumidity,
                        WindSpeed = item.DaytimeForecast.Wind.Speed.Value
                    });
                }
                return dailyForecast;
            }
            catch (JsonException e)
            {
                throw new ApiCallException($"failed to deserialize: {e.Message}.", inner: e);
            }
            catch (HttpRequestException e)
            {
                throw new ApiCallException($"failed to call googleweather: {e.Message}.", inner: e);
            }
        }
    }

    public class GoogleWeatherResponse
    {
        [JsonPropertyName("temperature")]
        public required Nested Tempreature { get; set; }

        public class Nested
        {
            [JsonPropertyName("degrees")]
            public decimal Degrees { get; set; }
        }
    }

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
