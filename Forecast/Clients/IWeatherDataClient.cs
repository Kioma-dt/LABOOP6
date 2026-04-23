namespace Forecast.Clients;

public interface IWeatherDataClient
{
    string Provider {  get; }
    Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude);
}
