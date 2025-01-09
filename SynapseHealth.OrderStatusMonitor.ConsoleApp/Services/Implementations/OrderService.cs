using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Models;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IAlertService _alertService;
        private readonly IUpdateService _updateService;

        public OrderService(IHttpClientService httpClientService, IAlertService alertService, IUpdateService updateService)
        {
            _httpClientService = httpClientService;
            _alertService = alertService;
            _updateService = updateService;
        }
        public async Task<List<MedicalEquipmentOrder>> FetchMedicalEquipmentOrdersAsync()
        {
            var response = await _httpClientService.GetAsync("https://orders-api.com/orders");
            response.EnsureSuccessStatusCode();
            var ordersData = await response.Content.ReadAsStringAsync();
            var ordersArray = JArray.Parse(ordersData).ToObject<List<MedicalEquipmentOrder>>();
            return ordersArray ?? new List<MedicalEquipmentOrder>();
        }

        public async Task ProcessMedicalEquipmentOrderAsync(MedicalEquipmentOrder order)
        {
            foreach (var item in order.MedicalEquipmentItems.Where(i => i.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase)))
            {
                var message = $"Alert for delivered item: Order {order.OrderId}, Item: {item.Description}";
                await _alertService.SendAlertAsync(message);
                item.DeliveryNotification++;
            }

            // Update the order after processing
            await _updateService.UpdateMedicalEquipmentOrderAsync(order);
        }
    }
}
