using Forecast.Models.Weather;
using Forecast.Shared.Responses.OpenWeather;
using Forecast.Utils;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http;
using System.Text.Json;

namespace Forecast.Clients
{
    public class OpenWeatherLocationDataClient : ILocationDataClient
    {
        private readonly HttpClient client;
        private readonly string apiKey;

        public OpenWeatherLocationDataClient(IConfiguration config, HttpClient httpClient) 
        {
            client = httpClient;
            client.BaseAddress = new Uri(config.GetValue<string>("OPENWEATHERLOCATION_BASE_URL") ?? "");
            apiKey = config.GetValue<string>("OPENWEATHER_API_KEY") ?? "";
        }
        public async Task<Location> CityLocation(string city, string countryCode)
        {
            try
            {
                var response = await client.GetAsync(
                    $"/direct?q={city},{countryCode}&limit=1&appid={apiKey}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiCallException(
                        $"openweather returned bad status: {(ushort)response.StatusCode}"
                    );
                }

                var data = await response.Content.ReadFromJsonAsync<List<OpenWeatherLocationResponse>>();

                if(data is null ||  data.Count == 0)
                {
                    throw new ApiCallException($"failed to decode response");
                }

                return new Location()
                { 
                    Latitude = data.First().Lat, 
                    Longitude = data.First().Lon
                };
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

        public async Task<IEnumerable<Location>> CityLocation(IEnumerable<City>? cities)
        {
            var locations = new List<Location>();

            if (cities is null)
            {
                throw new ApiCallException($"failed to decode response");
            }

            foreach (var city in cities)
            {
                var loca = await CityLocation(city.CityName, city.CountryCode);
                locations.Add(loca);
            }

            return locations;
        }
    }
}