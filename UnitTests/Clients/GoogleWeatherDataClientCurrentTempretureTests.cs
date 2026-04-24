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
    public class GoogleWeatherDataClientCurrentTempretureTests
    {
        [Fact]
        public async Task CorrectTemperature()
        {
            var json = """
                {
                    "temperature": {
                      "degrees": 30,
                      "unit": "CELSIUS"
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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.LocationCurrentTemperature(10, 20));
        }

        [Fact]
        public async Task WrongJsonFormat()
        {
            var json = """
                {

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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.LocationCurrentTemperature(10, 20));
        }

        [Fact]
        public async Task HttpRequestError()
        {
            var json = """
                {
                    "temperature": {
                          "degrees": 30,
                          "unit": "CELSIUS"
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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

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
                    Content = new StringContent("""{"temperature":{"degrees": 30,"unit": "CELSIUS"}}""")
                });

            var httpClient = new HttpClient(handler.Object);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl/",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

            await client.LocationCurrentTemperature(10, 20);

            var url = capturedRequest!.RequestUri!.ToString();

            Assert.Contains("http://someurl", url);
            Assert.Contains("/currentConditions:lookup?", url);
            Assert.Contains("location.latitude=10", url);
            Assert.Contains("location.longitude=20", url);
            Assert.Contains("key=somekey", url);
        }
    }
}
