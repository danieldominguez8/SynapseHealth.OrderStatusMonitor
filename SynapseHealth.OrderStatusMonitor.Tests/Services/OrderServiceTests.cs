using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
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
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _httpClientServiceMock = new Mock<IHttpClientService>();
            _alertServiceMock = new Mock<IAlertService>();
            _updateServiceMock = new Mock<IUpdateService>();
            _orderService = new OrderService(_httpClientServiceMock.Object, _alertServiceMock.Object, _updateServiceMock.Object);
        }

        [Fact]
        public async Task FetchMedicalEquipmentOrdersAsync_ReturnsOrders()
        {
            var orders = new List<MedicalEquipmentOrder>
            {
                new MedicalEquipmentOrder { OrderId = "1", MedicalEquipmentItems = new List<MedicalEquipmentItem>() }
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
        }

        [Fact]
        public async Task ProcessMedicalEquipmentOrderAsync_SendsAlertAndUpdatesOrder()
        {
            var order = new MedicalEquipmentOrder
            {
                OrderId = "1",
                MedicalEquipmentItems = new List<MedicalEquipmentItem>
                {
                    new MedicalEquipmentItem { Description = "TestItem", Status = "Delivered", DeliveryNotification = 0 }
                }
            };

            await _orderService.ProcessMedicalEquipmentOrderAsync(order);

            _alertServiceMock.Verify(x => x.SendAlertAsync(It.Is<string>(s => s.Contains("TestItem"))), Times.Once);
            _updateServiceMock.Verify(x => x.UpdateMedicalEquipmentOrderAsync(It.Is<MedicalEquipmentOrder>(o => o.OrderId == "1")), Times.Once);
            Assert.Equal(1, order.MedicalEquipmentItems[0].DeliveryNotification);
        }
    }
}
