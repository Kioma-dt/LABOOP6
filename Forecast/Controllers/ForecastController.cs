using Forecast.Clients;
using Forecast.Models.Weather;

namespace Forecast.Controllers
{
    public class ForecastController(IWeatherDataClientProvider clientProvider, ILocationDataClient locationClient)
    {
        private readonly IWeatherDataClientProvider clientProvider = clientProvider;
        private readonly ILocationDataClient locationClient = locationClient;

        public async Task<DailyForecast> GetDailyForecast(decimal latitude, decimal longitude, int days, string? provider = null)
        {
            return await clientProvider
                .GetWeatherDataClient(provider)
                .ForecastForDays(latitude, longitude, days);
        }

        public async Task<DailyForecast> GetDailyForecastByCity(string city, string countryCode, int days, string? provider = null)
        {
            var location = await locationClient.CityLocation(city, countryCode);

            return await GetDailyForecast(location.Latitude, location.Longitude, days, provider);
        }

    }
}
