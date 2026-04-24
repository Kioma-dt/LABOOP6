using Forecast.Clients;
using Forecast.Models.Weather;
using Forecast.Shared.Requstes;

namespace Forecast.Controllers;

public class CurrentWeatherController(IWeatherDataClientProvider clientProvider, ILocationDataClient locationClient)
{
    private readonly IWeatherDataClientProvider clientProvider = clientProvider;
    private readonly ILocationDataClient locationClient = locationClient;

    public async Task<CurrentWeather> GetCurrentWeather(decimal latitude, decimal longitude, string? provider = null)
    {
        var temperature = await clientProvider
            .GetWeatherDataClient(provider)
            .LocationCurrentTemperature(latitude, longitude);

        return new(temperature);
    }

    public async Task<IEnumerable<CurrentWeather>> GetCurrentWeather(MultipleLocationsRequest? request, string? provider = null)
    {
        return await clientProvider
            .GetWeatherDataClient(provider)
            .LocationCurrentTemperature(request?.Locations);
    }

    public async Task<CurrentWeather> GetCurrentWeatherByCity(string city, string countryCode, string? provider = null)
    {
       var location = await locationClient.CityLocation(city, countryCode);

        return await GetCurrentWeather(location.Latitude, location.Longitude, provider);
    }

    public async Task<IEnumerable<CurrentWeather>> GetCurrentWeatherByCity(MultipleCitiesRequest? request, string? provider = null)
    {
        var locations = await locationClient.CityLocation(request?.Cities);

        return await GetCurrentWeather(new MultipleLocationsRequest() { Locations = locations.ToList() }, provider);
    }
}
