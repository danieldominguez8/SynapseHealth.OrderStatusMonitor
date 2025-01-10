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
    public class UpdateService : IUpdateService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ILogger _logger;

        public UpdateService(IHttpClientService httpClientService, ILogger logger)
        {
            _httpClientService = httpClientService;
            _logger = logger;
        }

        public async Task UpdateMedicalEquipmentOrderAsync(MedicalEquipmentOrder order)
        {
            try
            {
                _logger.Information("Updating medical equipment order with ID: {OrderId}", order.OrderId);

                var content = new StringContent(order.ToString(), Encoding.UTF8, "application/json");
                var response = await _httpClientService.PutAsync($"http://localhost:5131/api/orders/{order.OrderId}", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("Successfully updated medical equipment order with ID: {OrderId}", order.OrderId);
                }
                else
                {
                    _logger.Error("Failed to update medical equipment order with ID: {OrderId}. Status Code: {StatusCode}", order.OrderId, response.StatusCode);
                }

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating medical equipment order with ID: {OrderId}", order.OrderId);
                throw;
            }
        }
    }
}
