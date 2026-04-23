namespace Forecast.Clients
{
    public interface IWeatherDataClientProvider
    {
        public IWeatherDataClient GetWeatherDataClient(string? apiProvider);
    }
    public class WeatherDataClientProvider : IWeatherDataClientProvider
    {
        public WeatherDataClientProvider(IEnumerable<IWeatherDataClient> clients)
        {
            throw new NotImplementedException();
        }
        public IWeatherDataClient GetWeatherDataClient(string? apiProvider)
        {
            throw new NotImplementedException();
        }
    }
}
