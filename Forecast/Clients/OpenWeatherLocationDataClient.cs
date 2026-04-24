using Forecast.Models.Weather;

namespace Forecast.Clients
{
    public class OpenWeatherLocationDataClient : ILocationDataClient
    {
        public Task<Location> CityLocation(string city, string countryCode)
        {
            throw new NotImplementedException();
        }
    }
}
