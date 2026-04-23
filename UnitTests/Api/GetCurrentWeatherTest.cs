using Forecast.Api;
using Forecast.Clients;
using Forecast.Controllers;
using Forecast.Models.Weather;
using Forecast.Shared.Responses;
using Forecast.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;


namespace Forecast.Tests.Api
{
    public class GetCurrentWeatherTest
    {
        [Fact]
        public async Task SuccessRequest()
        {
            var controllerMock = new Mock<CurrentWeatherController>(null!);

            controllerMock
                .Setup(x => x.GetCurrentWeather(10, 20))
                .ReturnsAsync(new CurrentWeather(25));

            var result = await WeatherApi.HandleGetCurrentWeather(
                controllerMock.Object,
                "10",
                "20"
            );

            Assert.True(result.Result is Ok<Success<CurrentWeather>>);
        }

        [Fact]
        public async Task InvalidParametars()
        {
            var controllerMock = new Mock<CurrentWeatherController>(null!);

            var result = await WeatherApi.HandleGetCurrentWeather(
                controllerMock.Object,
                "abc",
                "20"
            );

            Assert.True(result.Result is BadRequest<Status>);
        }

        [Fact]
        public async Task Overflow()
        {
            var controllerMock = new Mock<CurrentWeatherController>(null!);

            var result = await WeatherApi.HandleGetCurrentWeather(
                controllerMock.Object,
                "999999999999999",
                "20"
            );

            Assert.True(result.Result is BadRequest<Status>);
        }

        [Fact]
        public async Task ControllerError()
        {
            var controllerMock = new Mock<CurrentWeatherController>(null!);

            controllerMock
                .Setup(x => x.GetCurrentWeather(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ThrowsAsync(new ApiCallException());

            var result = await WeatherApi.HandleGetCurrentWeather(
                controllerMock.Object,
                "10",
                "20"
            );

            Assert.True(result.Result is InternalServerError<Status>);
        }
    }
}
