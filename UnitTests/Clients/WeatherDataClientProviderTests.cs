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
            var openWeather = new Mock<IWeatherDataClient>();
            var googleWeather = new Mock<IWeatherDataClient>();

            openWeather.Setup(x => x.Provider)
                .Returns("OpenWeather");
            googleWeather.Setup(x => x.Provider)
                .Returns("GoogleWeather");

            var clientProvider = new WeatherDataClientProvider( new List<IWeatherDataClient>
            {
                openWeather.Object,
                googleWeather.Object,
            });

            var client = clientProvider.GetWeatherDataClient("OpenWeather");

            Assert.Equal("OpenWeather", client.Provider);
        }

        [Fact]
        public void GoogleWeather()
        {
            var openWeather = new Mock<IWeatherDataClient>();
            var googleWeather = new Mock<IWeatherDataClient>();

            openWeather.Setup(x => x.Provider)
                .Returns("OpenWeather");
            googleWeather.Setup(x => x.Provider)
                .Returns("GoogleWeather");

            var clientProvider = new WeatherDataClientProvider(new List<IWeatherDataClient>
            {
                openWeather.Object,
                googleWeather.Object,
            });

            var client = clientProvider.GetWeatherDataClient("GoogleWeather");

            Assert.Equal("GoogleWeather", client.Provider);
        }

        [Fact]
        public void NullApiDeffaultOpenWearher()
        {
            var openWeather = new Mock<IWeatherDataClient>();
            var googleWeather = new Mock<IWeatherDataClient>();

            openWeather.Setup(x => x.Provider)
                .Returns("OpenWeather");
            googleWeather.Setup(x => x.Provider)
                .Returns("GoogleWeather");

            var clientProvider = new WeatherDataClientProvider(new List<IWeatherDataClient>
            {
                openWeather.Object,
                googleWeather.Object,
            });

            var client = clientProvider.GetWeatherDataClient(null);

            Assert.Equal("OpenWeather", client.Provider);
        }

        [Fact]
        public void UnknownApiException()
        {
            var openWeather = new Mock<IWeatherDataClient>();
            var googleWeather = new Mock<IWeatherDataClient>();

            openWeather.Setup(x => x.Provider)
                .Returns("OpenWeather");
            googleWeather.Setup(x => x.Provider)
                .Returns("GoogleWeather");

            var clientProvider = new WeatherDataClientProvider(new List<IWeatherDataClient>
            {
                openWeather.Object,
                googleWeather.Object,
            });


            Assert.Throws<ArgumentException>(() => clientProvider.GetWeatherDataClient("SomeApi"));
        }

        [Fact]
        public void NoClientsException()
        {

            var clientProvider = new WeatherDataClientProvider(new List<IWeatherDataClient>());


            Assert.Throws<ArgumentException>(() => clientProvider.GetWeatherDataClient("OpenWeather"));
        }
    }
}
