using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Serilog;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations;

namespace SynapseHealth.OrderStatusMonitor.Tests.Services
{
    public class HttpClientServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly HttpClientService _httpClientService;

        public HttpClientServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientService = new HttpClientService(httpClient, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAsync_ValidRequestUri_ReturnsHttpResponseMessageAndContent()
        {
            var requestUri = "https://orders-api.com/orders";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"OrderId\": \"1\", \"MedicalEquipmentItems\": [{\"Description\": \"Item1\", \"Status\": \"Delivered\", \"DeliveryNotification\": 1}]}")
            };
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == requestUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var result = await _httpClientService.GetAsync(requestUri);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            Assert.Contains("Item1", content);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == requestUri),
                ItExpr.IsAny<CancellationToken>()
            );
            _loggerMock.Verify(x => x.Information("Sending GET request to {RequestUri}", requestUri), Times.Once);
            _loggerMock.Verify(x => x.Information("GET request to {RequestUri} succeeded", requestUri), Times.Once);
        }

        [Fact]
        public async Task PostAsync_ValidRequestUriAndContent_ReturnsHttpResponseMessage()
        {
            var requestUri = "https://update-api.com/update";
            var content = new StringContent("Test content");
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"result\": \"success\"}")
            };
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString() == requestUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var result = await _httpClientService.PostAsync(requestUri, content);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseContent = await result.Content.ReadAsStringAsync();
            Assert.Contains("success", responseContent);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString() == requestUri),
                ItExpr.IsAny<CancellationToken>()
            );
            _loggerMock.Verify(x => x.Information("Sending POST request to {RequestUri}", requestUri), Times.Once);
            _loggerMock.Verify(x => x.Information("POST request to {RequestUri} succeeded", requestUri), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ExceptionThrown_LogsErrorAndRethrows()
        {
            var requestUri = "https://orders-api.com/orders";
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == requestUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _httpClientService.GetAsync(requestUri));
            Assert.Equal("Network error", exception.Message);
            _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), "An error occurred while sending GET request to {RequestUri}", requestUri), Times.Once);
        }

        [Fact]
        public async Task PostAsync_ExceptionThrown_LogsErrorAndRethrows()
        {
            var requestUri = "https://update-api.com/update";
            var content = new StringContent("Test content");
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString() == requestUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _httpClientService.PostAsync(requestUri, content));
            Assert.Equal("Network error", exception.Message);
            _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), "An error occurred while sending POST request to {RequestUri}", requestUri), Times.Once);
        }
    }
}
