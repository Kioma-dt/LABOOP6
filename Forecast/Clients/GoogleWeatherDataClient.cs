using Forecast.Utils;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json.Serialization;
using System.Text.Json;

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
                    $"?key={apiKey}&location.latitude={latitude}&location.longitude={longitude}"
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
}
