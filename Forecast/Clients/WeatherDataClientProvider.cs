namespace Forecast.Clients
{
    public interface IWeatherDataClientProvider
    {
        public IWeatherDataClient GetWeatherDataClient(string? apiProvider);
    }
    public class WeatherDataClientProvider : IWeatherDataClientProvider
    {
        readonly Dictionary<string, IWeatherDataClient> _clients;
        public WeatherDataClientProvider(IEnumerable<IWeatherDataClient> clients)
        {
            _clients = clients.ToDictionary(c => c.Provider);
        }
        public IWeatherDataClient GetWeatherDataClient(string? apiProvider)
        {
            if (_clients.Count == 0) 
            {
                throw new ArgumentException("No Clients Provided");
            }

            if (apiProvider is null)
            {
                var defaultClient = _clients.GetValueOrDefault("OpenWeather");
                return defaultClient ?? _clients.FirstOrDefault().Value;
            }

            if (!_clients.TryGetValue(apiProvider, out var client)) 
            {
                throw new ArgumentException($"No Client: {apiProvider}");
            }

            return client;
        }
    }
}
