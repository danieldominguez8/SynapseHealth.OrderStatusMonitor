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
    public class AlertService : IAlertService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ILogger _logger;

        public AlertService(IHttpClientService httpClientService, ILogger logger)
        {
            _httpClientService = httpClientService;
            _logger = logger;
        }

        public async Task SendAlertAsync(Alert alert)
        {
            try
            {
                _logger.Information("Sending alert with message: {Message}, timestamp: {Timestamp}",
                    alert.Message, alert.Timestamp);

                var content = new StringContent(
                    JObject.FromObject(alert).ToString(),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClientService.PostAsync("https://dandav8.mockmaster.io/synapsehealthmockapi/alerts", content); //mock api
                //var response = await _httpClientService.PostAsync("http://localhost:3000/alerts", content); //json-server api

                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("Successfully sent alert with message: {Message}", alert.Message);
                }
                else
                {
                    _logger.Error("Failed to send alert with message: {Message}. Status Code: {StatusCode}", alert.Message, response.StatusCode);
                }

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while sending alert with message: {Message}", alert.Message);
                throw;
            }
        }
    }
}
