using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.Tests.Services
{

    public class AlertServiceTests
    {
        private readonly Mock<IHttpClientService> _httpClientServiceMock;
        private readonly AlertService _alertService;

        public AlertServiceTests()
        {
            _httpClientServiceMock = new Mock<IHttpClientService>();
            _alertService = new AlertService(_httpClientServiceMock.Object);
        }

        [Fact]
        public async Task SendAlertAsync_ValidMessage_SendsAlert()
        {
            var message = "Test alert message";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _httpClientServiceMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(responseMessage);

            await _alertService.SendAlertAsync(message);

            _httpClientServiceMock.Verify(x => x.PostAsync(It.Is<string>(s => s == "https://alert-api.com/alerts"), It.IsAny<HttpContent>()), Times.Once);
        }

        [Fact]
        public async Task SendAlertAsync_HttpClientServiceReturnsError_ThrowsException()
        {
            // Arrange
            var message = "Test alert message";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            _httpClientServiceMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _alertService.SendAlertAsync(message));
        }
    }
}
