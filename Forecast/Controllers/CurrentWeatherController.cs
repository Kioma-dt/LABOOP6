using Forecast.Clients;
using Forecast.Models.Weather;
using Forecast.Shared.Requstes;

namespace Forecast.Controllers;

public class CurrentWeatherController(IWeatherDataClientProvider clientProvider)
{
    private readonly IWeatherDataClientProvider clientProvider = clientProvider;

    public async Task<CurrentWeather> GetCurrentWeather(decimal latitude, decimal longitude, string? provider = null)
    {
        var temperature = await clientProvider
            .GetWeatherDataClient(provider)
            .LocationCurrentTemperature(latitude, longitude);

        return new(temperature);
    }

    public async Task<IEnumerable<CurrentWeather>> GetCurrentWeather(MultipleLocationsRequest? locations, string? provider = null)
    {
        throw new NotImplementedException();
    }
}
