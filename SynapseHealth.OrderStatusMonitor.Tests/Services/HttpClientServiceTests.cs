using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations;

namespace SynapseHealth.OrderStatusMonitor.Tests.Services
{
    public class HttpClientServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClientService _httpClientService;

        public HttpClientServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientService = new HttpClientService(httpClient);
        }
        [Fact]
        public async Task GetAsync_ValidRequestUri_ReturnsHttpResponseMessageAndContent()
        {
            // Arrange
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

            // Act
            var result = await _httpClientService.GetAsync(requestUri);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            Assert.Contains("Item1", content);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == requestUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task PostAsync_ValidRequestUriAndContent_ReturnsHttpResponseMessage()
        {
            // Arrange
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

            // Act
            var result = await _httpClientService.PostAsync(requestUri, content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseContent = await result.Content.ReadAsStringAsync();
            Assert.Contains("success", responseContent);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString() == requestUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }


    }
}
