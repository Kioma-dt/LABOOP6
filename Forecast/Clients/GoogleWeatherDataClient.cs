using Forecast.Utils;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json.Serialization;
using System.Text.Json;
using Forecast.Models.Weather;
using Forecast.Shared.Responses.GoogleWeather;

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

                var data = await response.Content.ReadFromJsonAsync<GoogleWeatherTemretureResponse>();
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

        public async Task<IEnumerable<CurrentWeather>> LocationCurrentTemperature(IEnumerable<Location> locations)
        {
            var tempretures = new List<CurrentWeather>();
            foreach (var location in locations)
            {
                var temp = await LocationCurrentTemperature(location.Latitude, location.Longitude);
                tempretures.Add(new(temp));
            }

            return tempretures;
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
}
