using Xunit;
using Moq;

using Forecast.Controllers;
using Forecast.Clients;


namespace Forecast.Tests.Controllers
{
    public class CurrentWeatherControllerTests
    {
        [Fact]
        public async Task RettrunsCorrectTemperature()
        {
            var clientMock = new Mock<IWeatherDataClient>();

            clientMock.Setup(x => x.LocationCurrentTemperature(15.3m, 10.5m))
                .ReturnsAsync(25m);

            var controller = new CurrentWeatherController(clientMock.Object);

            var result = await controller.GetCurrentWeather(15.3m, 10.5m);

            Assert.Equal(25m, result.Temperature);
        }
    }
}
