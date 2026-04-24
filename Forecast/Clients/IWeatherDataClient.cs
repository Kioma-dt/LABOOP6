using Forecast.Models.Weather;
using Forecast.Shared.Requstes;
namespace Forecast.Clients;

public interface IWeatherDataClient
{
    string Provider {  get; }
    Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude);
    Task<IEnumerable<CurrentWeather>> LocationCurrentTemperature(IEnumerable<Location> locations);
    Task<DailyForecast> ForecastForDays(decimal latitude, decimal longitude, int days);
}
