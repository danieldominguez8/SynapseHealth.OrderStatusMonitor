using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Models;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IAlertService _alertService;
        private readonly IUpdateService _updateService;
        private readonly ILogger _logger; 

        public OrderService(IHttpClientService httpClientService, IAlertService alertService, IUpdateService updateService, ILogger logger)
        {
            _httpClientService = httpClientService;
            _alertService = alertService;
            _updateService = updateService;
            _logger = logger;
        }
        public async Task<List<MedicalEquipmentOrder>> FetchMedicalEquipmentOrdersAsync()
        {
            try
            {
                _logger.Information("Fetching medical equipment orders");
                var response = await _httpClientService.GetAsync("http://localhost:5131/api/orders");
                response.EnsureSuccessStatusCode();
                var ordersData = await response.Content.ReadAsStringAsync();
                var ordersArray = JArray.Parse(ordersData).ToObject<List<MedicalEquipmentOrder>>();

                if (ordersArray == null || !ordersArray.Any())
                {
                    _logger.Information("No medical equipment orders found");
                    return new List<MedicalEquipmentOrder>();
                }

                _logger.Information("Successfully fetched medical equipment orders");
                return ordersArray;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching medical equipment orders");
                throw;
            }
        }

        public async Task ProcessMedicalEquipmentOrderAsync(MedicalEquipmentOrder order)
        {
            try
            {
                _logger.Information("Processing medical equipment order with ID: {OrderId}", order.OrderId);
                foreach (var item in order.Items.Where(i => i.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase)))
                {
                    var message = $"Alert for delivered item: Order {order.OrderId}, Item: {item.Description}";
                    await _alertService.SendAlertAsync(message);
                    item.DeliveryNotification++;
                }

                await _updateService.UpdateMedicalEquipmentOrderAsync(order);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while processing medical equipment order with ID: {OrderId}", order.OrderId);
                throw;
            }
        }
    }
}
