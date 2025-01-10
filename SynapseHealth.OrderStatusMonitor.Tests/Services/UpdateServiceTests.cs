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
    public class UpdateServiceTests
    {
        private readonly Mock<IHttpClientService> _httpClientServiceMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly UpdateService _updateService;

        public UpdateServiceTests()
        {
            _httpClientServiceMock = new Mock<IHttpClientService>();
            _loggerMock = new Mock<ILogger>();
            _updateService = new UpdateService(_httpClientServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UpdateMedicalEquipmentOrderAsync_ValidOrder_UpdatesOrder()
        {
            var order = new MedicalEquipmentOrder { id = "1", Items = new List<MedicalEquipmentItem>() };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"result\": \"success\"}")
            };
            _httpClientServiceMock.Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(responseMessage);

            await _updateService.UpdateMedicalEquipmentOrderAsync(order);

            _httpClientServiceMock.Verify(x => x.PutAsync(It.Is<string>(s => s == "http://localhost:3000/orders/1"), It.IsAny<HttpContent>()), Times.Once);
            _loggerMock.Verify(x => x.Information("Successfully updated medical equipment order with ID: {id}", "1"), Times.Once);
        }

        [Fact]
        public async Task UpdateMedicalEquipmentOrderAsync_HttpClientServiceReturnsError_ThrowsException()
        {
            var order = new MedicalEquipmentOrder { id = "1", Items = new List<MedicalEquipmentItem>() };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            _httpClientServiceMock.Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(responseMessage);

            await Assert.ThrowsAsync<HttpRequestException>(() => _updateService.UpdateMedicalEquipmentOrderAsync(order));
            _loggerMock.Verify(x => x.Error("Failed to update medical equipment order with ID: {id}. Status Code: {StatusCode}", "1", HttpStatusCode.BadRequest), Times.Once);
        }

        [Fact]
        public async Task UpdateMedicalEquipmentOrderAsync_ExceptionThrown_LogsErrorAndRethrows()
        {
            var order = new MedicalEquipmentOrder { id = "1", Items = new List<MedicalEquipmentItem>() };
            _httpClientServiceMock.Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ThrowsAsync(new HttpRequestException("Network error"));

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _updateService.UpdateMedicalEquipmentOrderAsync(order));
            Assert.Equal("Network error", exception.Message);
            _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), "An error occurred while updating medical equipment order with ID: {id}", "1"), Times.Once);
        }
    }
}
