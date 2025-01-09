using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations
{
    public class AlertService : IAlertService
    {
        private readonly IHttpClientService _httpClientService;

        public AlertService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task SendAlertAsync(string message)
        {
            var content = new StringContent(
                JObject.FromObject(new { Message = message }).ToString(),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClientService.PostAsync("https://alert-api.com/alerts", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
