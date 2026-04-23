using Xunit;
using Moq;

using Forecast.Controllers;
using Forecast.Clients;


namespace Forecast.Tests.Controllers
{
    public class CurrentWeatherControllerTests
    {
        [Fact]
        public async Task CorrectTemperature()
        {
            var clientProviderMock = new Mock<IWeatherDataClientProvider>();
            var clientMock = new Mock<IWeatherDataClient>();

            clientMock.Setup(x => x.LocationCurrentTemperature(15.3m, 10.5m))
                .ReturnsAsync(25m);

            clientProviderMock.Setup(x => x.GetWeatherDataClient(null))
                .Returns(clientMock.Object);

            var controller = new CurrentWeatherController(clientProviderMock.Object);

            var result = await controller.GetCurrentWeather(15.3m, 10.5m);

            Assert.Equal(25m, result.Temperature);
        }
    }
}
