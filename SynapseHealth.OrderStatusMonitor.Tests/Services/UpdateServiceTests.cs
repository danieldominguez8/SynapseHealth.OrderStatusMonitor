using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Models;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.Tests.Services
{
    public class UpdateServiceTests
    {
        private readonly Mock<IHttpClientService> _httpClientServiceMock;
        private readonly UpdateService _updateService;

        public UpdateServiceTests()
        {
            _httpClientServiceMock = new Mock<IHttpClientService>();
            _updateService = new UpdateService(_httpClientServiceMock.Object);
        }

        [Fact]
        public async Task UpdateMedicalEquipmentOrderAsync_ValidOrder_UpdatesOrder()
        {
            // Arrange
            var order = new MedicalEquipmentOrder { OrderId = "1", MedicalEquipmentItems = new List<MedicalEquipmentItem>() };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _httpClientServiceMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(responseMessage);

            // Act
            await _updateService.UpdateMedicalEquipmentOrderAsync(order);

            // Assert
            _httpClientServiceMock.Verify(x => x.PostAsync(It.Is<string>(s => s == "https://update-api.com/update"), It.IsAny<HttpContent>()), Times.Once);
        }

        [Fact]
        public async Task UpdateMedicalEquipmentOrderAsync_HttpClientServiceReturnsError_ThrowsException()
        {
            // Arrange
            var order = new MedicalEquipmentOrder { OrderId = "1", MedicalEquipmentItems = new List<MedicalEquipmentItem>() };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            _httpClientServiceMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>())).ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _updateService.UpdateMedicalEquipmentOrderAsync(order));
        }
    }
}
