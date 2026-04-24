using Forecast.Models.Weather;

namespace Forecast.Clients
{
    public interface ILocationDataClient
    {
        Task<Location> CityLocation(string city, string countryCode);

        Task<IEnumerable<Location>> CityLocation(IEnumerable<City>? cities);
    }
}
