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
    public class OpenWeatherDataClientTests
    {
        [Fact]
        public async Task CorrectTemperature()
        {
            var json = """
                {
                    "main": {
                        "temp": 30
                    }
                }
                """;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var handler = new MyHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHER_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherDataClient(configuration, httpClient);

            var temp = await client.LocationCurrentTemperature(10, 20);

            Assert.Equal(30, temp);
        }

        [Fact]
        public async Task BadRequestStatucCodeFromServer()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            var handler = new MyHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHER_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.LocationCurrentTemperature(10, 20));
        }

        [Fact]
        public async Task HttpRequestError()
        {
            var json = """
                {
                    "main": {
                        "temp": 30
                    }
                }
                """;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var handler = new MyHttpMessageHandler(response, true);
            var httpClient = new HttpClient(handler);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHER_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.LocationCurrentTemperature(10, 20));
        }

        [Fact]
        public async Task CorrectRequsetUrl()
        {
            HttpRequestMessage? capturedRequest = null;

            var handler = new Mock<HttpMessageHandler>();

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{"main":{"temp":30}}""")
                });

            var httpClient = new HttpClient(handler.Object);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHER_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherDataClient(configuration, httpClient);

            await client.LocationCurrentTemperature(10, 20);

            Assert.Contains("http://someurl", capturedRequest!.RequestUri!.ToString());
            Assert.Contains("lat=10", capturedRequest!.RequestUri!.ToString());
            Assert.Contains("lon=20", capturedRequest.RequestUri.ToString());
            Assert.Contains("appid=somekey", capturedRequest.RequestUri.ToString());
        }
    }
}



    public class MyHttpMessageHandler : HttpMessageHandler
    {
        readonly HttpResponseMessage response;
        readonly bool except;

        public MyHttpMessageHandler(HttpResponseMessage response, bool except = false)
        {
            this.response = response;
            this.except = except;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (except)
            {
                throw new HttpRequestException();
            }

            return Task.FromResult(response);
        }
    }
