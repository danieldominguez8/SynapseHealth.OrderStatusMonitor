using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog;
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

        public async Task SendAlertAsync(string message)
        {
            try
            {
                _logger.Information("Sending alert with message: {Message}", message);

                var content = new StringContent(
                    JObject.FromObject(new { Message = message }).ToString(),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClientService.PostAsync("http://localhost:5131/api/alerts", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("Successfully sent alert with message: {Message}", message);
                }
                else
                {
                    _logger.Error("Failed to send alert with message: {Message}. Status Code: {StatusCode}", message, response.StatusCode);
                }

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while sending alert with message: {Message}", message);
                throw;
            }
        }
    }
}
