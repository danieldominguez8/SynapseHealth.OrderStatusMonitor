using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Serilog;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Models;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;
using Xunit;

namespace SynapseHealth.OrderStatusMonitor.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IHttpClientService> _httpClientServiceMock;
        private readonly Mock<IAlertService> _alertServiceMock;
        private readonly Mock<IUpdateService> _updateServiceMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _httpClientServiceMock = new Mock<IHttpClientService>();
            _alertServiceMock = new Mock<IAlertService>();
            _updateServiceMock = new Mock<IUpdateService>();
            _loggerMock = new Mock<ILogger>();
            _orderService = new OrderService(_httpClientServiceMock.Object, _alertServiceMock.Object, _updateServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task FetchMedicalEquipmentOrdersAsync_ReturnsOrders()
        {
            var orders = new List<MedicalEquipmentOrder>
        {
            new MedicalEquipmentOrder { OrderId = "1", Items = new List<MedicalEquipmentItem>() }
        };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(orders))
            };
            _httpClientServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(responseMessage);

            var result = await _orderService.FetchMedicalEquipmentOrdersAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("1", result[0].OrderId);
            _loggerMock.Verify(x => x.Information("Fetching medical equipment orders"), Times.Once);
            _loggerMock.Verify(x => x.Information("Successfully fetched medical equipment orders"), Times.Once);
        }

        [Fact]
        public async Task FetchMedicalEquipmentOrdersAsync_NoOrdersFound_LogsMessage()
        {
            var orders = new List<MedicalEquipmentOrder>();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(orders))
            };
            _httpClientServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(responseMessage);

            var result = await _orderService.FetchMedicalEquipmentOrdersAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
            _loggerMock.Verify(x => x.Information("Fetching medical equipment orders"), Times.Once);
            _loggerMock.Verify(x => x.Information("No medical equipment orders found"), Times.Once);
        }

        [Fact]
        public async Task ProcessMedicalEquipmentOrderAsync_SendsAlertAndUpdatesOrder()
        {
            var order = new MedicalEquipmentOrder
            {
                OrderId = "1",
                Items = new List<MedicalEquipmentItem>
            {
                new MedicalEquipmentItem { Description = "TestItem", Status = "Delivered", DeliveryNotification = 0 }
            }
            };

            await _orderService.ProcessMedicalEquipmentOrderAsync(order);

            _alertServiceMock.Verify(x => x.SendAlertAsync(It.Is<string>(s => s.Contains("TestItem"))), Times.Once);
            _updateServiceMock.Verify(x => x.UpdateMedicalEquipmentOrderAsync(It.Is<MedicalEquipmentOrder>(o => o.OrderId == "1")), Times.Once);
            Assert.Equal(1, order.Items[0].DeliveryNotification);
            _loggerMock.Verify(x => x.Information("Processing medical equipment order with ID: {OrderId}", "1"), Times.Once);
        }

        [Fact]
        public async Task FetchMedicalEquipmentOrdersAsync_ExceptionThrown_LogsErrorAndRethrows()
        {
            _httpClientServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ThrowsAsync(new HttpRequestException("Network error"));

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _orderService.FetchMedicalEquipmentOrdersAsync());
            Assert.Equal("Network error", exception.Message);
            _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), "An error occurred while fetching medical equipment orders"), Times.Once);
        }

        [Fact]
        public async Task ProcessMedicalEquipmentOrderAsync_ExceptionThrown_LogsErrorAndRethrows()
        {
            var order = new MedicalEquipmentOrder
            {
                OrderId = "1",
                Items = new List<MedicalEquipmentItem>
            {
                new MedicalEquipmentItem { Description = "TestItem", Status = "Delivered", DeliveryNotification = 0 }
            }
            };
            _updateServiceMock.Setup(x => x.UpdateMedicalEquipmentOrderAsync(It.IsAny<MedicalEquipmentOrder>())).ThrowsAsync(new HttpRequestException("Network error"));

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _orderService.ProcessMedicalEquipmentOrderAsync(order));
            Assert.Equal("Network error", exception.Message);
            _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), "An error occurred while processing medical equipment order with ID: {OrderId}", "1"), Times.Once);
        }
    }
}
