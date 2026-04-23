using Forecast.Models.Weather;
namespace Forecast.Clients;

public interface IWeatherDataClient
{
    string Provider {  get; }
    Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude);
    Task<DailyForecast> ForecastForDays(decimal latitude, decimal longitude, int days);
}
