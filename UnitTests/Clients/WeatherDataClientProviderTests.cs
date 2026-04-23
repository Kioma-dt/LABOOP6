using Forecast.Clients;
using Forecast.Controllers;
using Forecast.Utils;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace Forecast.Tests.Clients
{
    public class WeatherDataClientProviderTests
    {
        [Fact]
        public void OpenWeather()
        {
            var clientProvider = new WeatherDataClientProvider( new List<IWeatherDataClient>
            {
                new Mock<OpenWeatherDataClient>().Object,
                new Mock<GoogleWeatherDataClient>().Object,
            });

            var client = clientProvider.GetWeatherDataClient("OpenWeather");

            Assert.IsType<OpenWeatherDataClient>(client);
        }

        [Fact]
        public void GoogleWeather()
        {
            var clientProvider = new WeatherDataClientProvider(new List<IWeatherDataClient>
            {
                new Mock<OpenWeatherDataClient>().Object,
                new Mock<GoogleWeatherDataClient>().Object,
            });

            var client = clientProvider.GetWeatherDataClient("GoogleWeather");

            Assert.IsType<GoogleWeatherDataClient>(client);
        }

        [Fact]
        public void NullApi()
        {
            var clientProvider = new WeatherDataClientProvider(new List<IWeatherDataClient>
            {
                new Mock<OpenWeatherDataClient>().Object,
                new Mock<GoogleWeatherDataClient>().Object,
            });

            var client = clientProvider.GetWeatherDataClient(null);

            Assert.IsType<OpenWeatherDataClient>(client);
        }

        [Fact]
        public void UnknownApi()
        {
            var clientProvider = new WeatherDataClientProvider(new List<IWeatherDataClient>
            {
                new Mock<OpenWeatherDataClient>().Object,
                new Mock<GoogleWeatherDataClient>().Object,
            });


            Assert.Throws<ArgumentException>(() => clientProvider.GetWeatherDataClient("SomeApi"));
        }
    }
}
