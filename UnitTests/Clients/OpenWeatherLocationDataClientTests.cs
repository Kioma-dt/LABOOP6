using Forecast.Clients;
using Forecast.Utils;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Forecast.Tests.Clients
{
    public class OpenWeatherLocationDataClientTests
    {
        [Fact]
        public async Task CorrectLocation()
        {
            var json = """
                [
                {
                  "name": "London",
                  "lat": 10,
                  "lon": 20,
                  "country": "GB"
                }
                ]
                """;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var handler = new MyHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHERLOCATION_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherLocationDataClient(configuration, httpClient);

            var location = await client.CityLocation("London", "GB");

            Assert.Equal(10, location.Latitude);
            Assert.Equal(20, location.Longitude);
        }

        [Fact]
        public async Task MultipleLocationsNullRequest()
        {
            var json = """
                [
                {
                  "name": "London",
                  "lat": 10,
                  "lon": 20,
                  "country": "GB"
                }
                ]
                """;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var handler = new MyHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHERLOCATION_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherLocationDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.CityLocation(null));
        }

        [Fact]
        public async Task BadRequestStatucCodeFromServer()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            var handler = new MyHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHERLOCATION_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherLocationDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.CityLocation("London", "GB"));
        }

        [Fact]
        public async Task WrongJsonFormat()
        {
            var json = """
                {
                    "somejson" : 1
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
                ["OPENWEATHERLOCATION_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherLocationDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.CityLocation("London", "GB"));
        }

        [Fact]
        public async Task HttpRequestError()
        {
            var json = """
                [
                    {
                      "name": "London",
                      "lat": 10,
                      "lon": 20,
                      "country": "GB"
                    }
                ]
                """;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var handler = new MyHttpMessageHandler(response, true);
            var httpClient = new HttpClient(handler);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHERLOCATION_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherLocationDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.CityLocation("London", "GB"));
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
                    Content = new StringContent("""
                        [
                            {
                              "name": "London",
                              "lat": 10,
                              "lon": 20,
                              "country": "GB"
                            }
                        ]
                        """)
                });

            var httpClient = new HttpClient(handler.Object);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["OPENWEATHERLOCATION_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherLocationDataClient(configuration, httpClient);

            await client.CityLocation("London", "GB");

            var url = capturedRequest!.RequestUri!.ToString().ToLower();

            Assert.Contains("http://someurl", url);
            Assert.Contains("/direct?", url);
            Assert.Contains("q=london,gb", url);
            Assert.Contains("appid=somekey", url);
        }
    }
}
