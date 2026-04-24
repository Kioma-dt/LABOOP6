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
    public class OpenWeatherDataClientForecastTests
    {
        [Fact]
        public async Task CorrectForecast()
        {
            var json = """
            {
              "list": [
                {
                  "dt": 1661857200,
                  "temp": {
                    "min": 288.93,
                    "max": 299.66
                  },
                  "humidity": 44,
                  "speed": 2.7
                },
                {
                  "dt": 1661943600,
                  "temp": {
                    "min": 287.73,
                    "max": 295.76
                  },
                  "humidity": 60,
                  "speed": 2.29
                }
              ]
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

            var dailyForecast = await client.ForecastForDays(10, 20, 2);

            Assert.NotNull(dailyForecast);
            Assert.Equal(2, dailyForecast.DayForecasts.Count);

            var day1 = dailyForecast.DayForecasts[0];
            Assert.Equal(
                DateTimeOffset.FromUnixTimeSeconds(1661857200).UtcDateTime,
                day1.Date
            );
            Assert.Equal(288.93m, day1.MinTempreture);
            Assert.Equal(299.66m, day1.MaxTempreture);
            Assert.Equal(44m, day1.Humidity);
            Assert.Equal(2.7m, day1.WindSpeed);

            var day2 = dailyForecast.DayForecasts[1];
            Assert.Equal(
                DateTimeOffset.FromUnixTimeSeconds(1661943600).UtcDateTime,
                day2.Date
            );
            Assert.Equal(287.73m, day2.MinTempreture);
            Assert.Equal(295.76m, day2.MaxTempreture);
            Assert.Equal(60m, day2.Humidity);
            Assert.Equal(2.29m, day2.WindSpeed);
        }

        [Fact]
        public async Task EmptyForecast()
        {
            var json = """
            {
              "list": []
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

            var dailyForecast = await client.ForecastForDays(10, 20, 0);

            Assert.Empty(dailyForecast.DayForecasts);
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

            await Assert.ThrowsAsync<ApiCallException>(() => client.ForecastForDays(10, 20, 2));
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
                ["OPENWEATHER_BASE_URL"] = "http://someurl",
                ["OPENWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new OpenWeatherDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.ForecastForDays(10, 20, 2));
        }

        [Fact]
        public async Task HttpRequestError()
        {
            var json = """
            {
              "list": [
                {
                  "dt": 1661857200,
                  "temp": {
                    "min": 288.93,
                    "max": 299.66
                  },
                  "humidity": 44,
                  "speed": 2.7
                },
                {
                  "dt": 1661943600,
                  "temp": {
                    "min": 287.73,
                    "max": 295.76
                  },
                  "humidity": 60,
                  "speed": 2.29
                }
              ]
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

            await Assert.ThrowsAsync<ApiCallException>(() => client.ForecastForDays(10, 20, 2));
        }

        [Fact]
        public async Task CorrectForecastRequestUrl()
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
                        {
                          "list": []
                        }
                        """)
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

            await client.ForecastForDays(10, 20, 2);

            var url = capturedRequest!.RequestUri!.ToString();

            Assert.Contains("http://someurl", url);
            Assert.Contains("lat=10", url);
            Assert.Contains("lon=20", url);
            Assert.Contains("cnt=2", url);
            Assert.Contains("appid=somekey", url);
        }
    }
    public class OpenWeatherDataClientCurrentTempretureTests
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

            var url = capturedRequest!.RequestUri!.ToString();

            Assert.Contains("http://someurl", url);
            Assert.Contains("lat=10", url);
            Assert.Contains("lon=20", url);
            Assert.Contains("appid=somekey", url);
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
