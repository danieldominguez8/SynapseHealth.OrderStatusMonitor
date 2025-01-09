using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IAlertService _alertService;
        private readonly IUpdateService _updateService;

        public OrderService(HttpClient httpClient, IAlertService alertService, IUpdateService updateService)
        {
            _httpClient = httpClient;
            _alertService = alertService;
            _updateService = updateService;
        }
        public async Task<JObject[]> FetchMedicalEquipmentOrdersAsync()
        {
            var response = await _httpClient.GetAsync("https://orders-api.com/orders");
            response.EnsureSuccessStatusCode();
            var ordersData = await response.Content.ReadAsStringAsync();
            var ordersArray = JArray.Parse(ordersData).ToObject<JObject[]>();
            return ordersArray ?? Array.Empty<JObject>();
        }

        public async Task ProcessMedicalEquipmentOrderAsync(JObject medicalEquipmentOrder)
        {
            foreach (var item in medicalEquipmentOrder["Items"].ToObject<JArray>())
            {
                if (IsItemDelivered(item))
                {
                    var message = $"Alert for delivered item: Order {medicalEquipmentOrder["OrderId"]}, Item: {item["Description"]}";
                    await _alertService.SendAlertAsync(message);
                    IncrementDeliveryNotification(item);
                }
            }

            await _updateService.UpdateMedicalEquipmentOrderAsync(medicalEquipmentOrder);
        }

        private bool IsItemDelivered(JToken item) => item["Status"].ToString().Equals("Delivered", StringComparison.OrdinalIgnoreCase);

        private void IncrementDeliveryNotification(JToken item) => item["deliveryNotification"] = item["deliveryNotification"].Value<int>() + 1;
    }
}
