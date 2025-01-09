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
    public class UpdateService : IUpdateService
    {
        private readonly HttpClient _httpClient;

        public UpdateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UpdateMedicalEquipmentOrderAsync(MedicalEquipmentOrder order)
        {
            var content = new StringContent(order.ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://update-api.com/update", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
