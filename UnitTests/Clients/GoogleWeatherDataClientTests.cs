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
    public class GoogleWeatherDataClientForecastTests
    {
        [Fact]
        public async Task CorrectForecast()
        {
            var json = """
            {
              "forecastDays": [
                {
                  "displayDate": {
                    "year": 2025,
                    "month": 2,
                    "day": 10
                  },
                  "daytimeForecast": {
                    "relativeHumidity": 54,
                    "wind": {
                      "speed": {
                        "value": 6
                      }
                    }
                  },
                  "maxTemperature": {
                    "degrees": 13.3
                  },
                  "minTemperature": {
                    "degrees": 1.5
                  }
                },
                {
                  "displayDate": {
                    "year": 2025,
                    "month": 2,
                    "day": 11
                  },
                  "daytimeForecast": {
                    "relativeHumidity": 75,
                    "wind": {
                      "speed": {
                        "value": 10
                      }
                    }
                  },
                  "maxTemperature": {
                    "degrees": 15.0
                  },
                  "minTemperature": {
                    "degrees": 5.0
                  }
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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

            var dailyForecast = await client.ForecastForDays(10, 20, 2);

            Assert.NotNull(dailyForecast);
            Assert.Equal(2, dailyForecast.DayForecasts.Count);

            var day1 = dailyForecast.DayForecasts[0];
            Assert.Equal(new DateTime(2025, 2, 10), day1.Date);
            Assert.Equal(1.5m, day1.MinTempreture);
            Assert.Equal(13.3m, day1.MaxTempreture);
            Assert.Equal(54m, day1.Humidity);
            Assert.Equal(6m, day1.WindSpeed);

            var day2 = dailyForecast.DayForecasts[1];
            Assert.Equal(new DateTime(2025, 2, 11), day2.Date);
            Assert.Equal(5.0m, day2.MinTempreture);
            Assert.Equal(15.0m, day2.MaxTempreture);
            Assert.Equal(75m, day2.Humidity);
            Assert.Equal(10m, day2.WindSpeed);
        }

        [Fact]
        public async Task EmptyForecast()
        {
            var json = """
            {
              "forecastDays": []
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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

            await Assert.ThrowsAsync<ApiCallException>(() => client.ForecastForDays(10, 20, 2));
        }

        [Fact]
        public async Task HttpRequestError()
        {
            var json = """
            {
              "forecastDays": [
                {
                  "displayDate": {
                    "year": 2025,
                    "month": 2,
                    "day": 10
                  },
                  "daytimeForecast": {
                    "relativeHumidity": 54,
                    "wind": {
                      "speed": {
                        "value": 6
                      }
                    }
                  },
                  "maxTemperature": {
                    "degrees": 13.3
                  },
                  "minTemperature": {
                    "degrees": 1.5
                  }
                },
                {
                  "displayDate": {
                    "year": 2025,
                    "month": 2,
                    "day": 11
                  },
                  "daytimeForecast": {
                    "relativeHumidity": 75,
                    "wind": {
                      "speed": {
                        "value": 10
                      }
                    }
                  },
                  "maxTemperature": {
                    "degrees": 15.0
                  },
                  "minTemperature": {
                    "degrees": 5.0
                  }
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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

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
                          "forecastDays": []
                        }
                        """)
                });

            var httpClient = new HttpClient(handler.Object);

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

            await client.ForecastForDays(10, 20, 2);

            var url = capturedRequest!.RequestUri!.ToString();

            Assert.Contains("http://someurl", url);
            Assert.Contains("location.latitude=10", url);
            Assert.Contains("location.longitude=20", url);
            Assert.Contains("days=2", url);
            Assert.Contains("key=somekey", url);
        }
    }
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
                ["GOOGLEWEATHER_BASE_URL"] = "http://someurl",
                ["GOOGLEWEATHER_API_KEY"] = "somekey"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var client = new GoogleWeatherDataClient(configuration, httpClient);

            await client.LocationCurrentTemperature(10, 20);

            var url = capturedRequest!.RequestUri!.ToString();

            Assert.Contains("http://someurl", url);
            Assert.Contains("location.latitude=10", url);
            Assert.Contains("location.longitude=20", url);
            Assert.Contains("key=somekey", url);
        }
    }
}
