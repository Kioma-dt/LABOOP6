using System.Text.Json;
using System.Text.Json.Serialization;
using Forecast.Models.Weather;
using Forecast.Utils;
using Forecast.Shared.Responses.OpenWeather;

namespace Forecast.Clients;

public class OpenWeatherDataClient : IWeatherDataClient
{
    private readonly HttpClient client;
    private readonly string apiKey;

    public OpenWeatherDataClient(IConfiguration config, HttpClient httpClient)
    {
        client = httpClient;
        client.BaseAddress = new Uri(config.GetValue<string>("OPENWEATHER_BASE_URL") ?? "");
        apiKey = config.GetValue<string>("OPENWEATHER_API_KEY") ?? "";
    }

    public string Provider => "OpenWeather";

    public async Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
    {
        try
        {
            var response = await client.GetAsync(
                $"?/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric"
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiCallException(
                    $"openweather returned bad status: {(ushort)response.StatusCode}"
                );
            }

            var data = await response.Content.ReadFromJsonAsync<OpenWeatherTemretureResponse>();
            return data?.Main?.Temp ?? throw new ApiCallException($"failed to decode response");
        }
        catch (JsonException e)
        {
            throw new ApiCallException($"failed to deserialize: {e.Message}.", inner: e);
        }
        catch (HttpRequestException e)
        {
            throw new ApiCallException($"failed to call openweather: {e.Message}.", inner: e);
        }
    }

    public Task<IEnumerable<CurrentWeather>> LocationCurrentTemperature(IEnumerable<Location> locations)
    {
        throw new NotImplementedException();
    }

    public async Task<DailyForecast> ForecastForDays(decimal latitude, decimal longitude, int days)
    {
        try
        {
            var response = await client.GetAsync(
                $"/forecast/daily?lat={latitude}&lon={longitude}&cnt={days}&appid={apiKey}&units=metric"
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiCallException(
                    $"openweather returned bad status: {(ushort)response.StatusCode}"
                );
            }

            var data = await response.Content.ReadFromJsonAsync<OpenWeatherForecastResponse>();

            if (data is null || data.List is null)
            {
                throw new ApiCallException($"failed to decode response");
            }

            var dailyForecast = new DailyForecast();

            foreach (var item in data.List) 
            {
                dailyForecast.DayForecasts.Add(new DayForecast()
                {
                    Date = DateTimeOffset.FromUnixTimeSeconds(item.Date).UtcDateTime,
                    MaxTempreture = item.Temperature.Max,
                    MinTempreture = item.Temperature.Min,
                    Humidity = item.Humidity,
                    WindSpeed = item.Speed
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
            throw new ApiCallException($"failed to call openweather: {e.Message}.", inner: e);
        }
    }

}