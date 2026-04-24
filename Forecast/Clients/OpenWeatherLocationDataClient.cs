using Forecast.Models.Weather;

namespace Forecast.Clients
{
    public class OpenWeatherLocationDataClient : ILocationDataClient
    {
        public OpenWeatherLocationDataClient(IConfiguration config, HttpClient client) 
        {
            throw new NotImplementedException();
        }
        public Task<Location> CityLocation(string city, string countryCode)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Location>> CityLocation(IEnumerable<City>? cities)
        {
            throw new NotImplementedException();
        }
    }
}