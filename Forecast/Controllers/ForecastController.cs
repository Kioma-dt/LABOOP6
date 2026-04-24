using Forecast.Clients;
using Forecast.Models.Weather;

namespace Forecast.Controllers
{
    public class ForecastController(IWeatherDataClientProvider clientProvider)
    {
        private readonly IWeatherDataClientProvider clientProvider = clientProvider;

        public async Task<DailyForecast> GetDailyForecast(decimal latitude, decimal longitude, int days, string? provider = null)
        {
            return await clientProvider
                .GetWeatherDataClient(provider)
                .ForecastForDays(latitude, longitude, days);
        }
    }
}
