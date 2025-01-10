using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Serilog;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Models;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.Tests.Services
{

    public class AlertServiceTests
    {
        private readonly Mock<IHttpClientService> _httpClientServiceMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly AlertService _alertService;

        public AlertServiceTests()
        {
            _httpClientServiceMock = new Mock<IHttpClientService>();
            _loggerMock = new Mock<ILogger>();
            _alertService = new AlertService(_httpClientServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SendAlertAsync_ValidAlert_SendsAlert()
        {
            var alert = new Alert
            {
                Message = "Test alert message",
                Timestamp = DateTime.UtcNow
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"result\": \"success\"}")
            };
            _httpClientServiceMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(responseMessage);

            await _alertService.SendAlertAsync(alert);

            _httpClientServiceMock.Verify(x => x.PostAsync(It.Is<string>(s => s == "http://localhost:3000/alerts"), It.IsAny<HttpContent>()), Times.Once);
            _loggerMock.Verify(x => x.Information("Successfully sent alert with message: {Message}", alert.Message), Times.Once);
        }


        [Fact]
        public async Task SendAlertAsync_HttpClientServiceReturnsError_ThrowsException()
        {
            var alert = new Alert
            {
                Message = "Test alert message",
                Timestamp = DateTime.UtcNow
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            _httpClientServiceMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(responseMessage);

            await Assert.ThrowsAsync<HttpRequestException>(() => _alertService.SendAlertAsync(alert));
            _loggerMock.Verify(x => x.Error("Failed to send alert with message: {Message}. Status Code: {StatusCode}", alert.Message, HttpStatusCode.BadRequest), Times.Once);
        }

        [Fact]
        public async Task SendAlertAsync_ExceptionThrown_LogsErrorAndRethrows()
        {
            var alert = new Alert
            {
                Message = "Test alert message",
                Timestamp = DateTime.UtcNow
            };
            _httpClientServiceMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ThrowsAsync(new HttpRequestException("Network error"));

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _alertService.SendAlertAsync(alert));
            Assert.Equal("Network error", exception.Message);
            _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), "An error occurred while sending alert with message: {Message}", alert.Message), Times.Once);
        }
    }
}
