
namespace Forecast.Clients
{
    public class GoogleWeatherDataClient : IWeatherDataClient
    {
        public string Provider => throw new NotImplementedException();

        public Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
        {
            throw new NotImplementedException();
        }
    }
}
