using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public HttpClientService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            try
            {
                _logger.Information("Sending GET request to {RequestUri}", requestUri);
                var response = await _httpClient.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("GET request to {RequestUri} succeeded", requestUri);
                }
                else
                {
                    _logger.Error("GET request to {RequestUri} failed with status code {StatusCode}", requestUri, response.StatusCode);
                }
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.Error(ex, "An error occurred while sending GET request to {RequestUri}", requestUri);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            try
            {
                _logger.Information("Sending POST request to {RequestUri}", requestUri);
                var response = await _httpClient.PostAsync(requestUri, content);
                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("POST request to {RequestUri} succeeded", requestUri);
                }
                else
                {
                    _logger.Error("POST request to {RequestUri} failed with status code {StatusCode}", requestUri, response.StatusCode);
                }
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.Error(ex, "An error occurred while sending POST request to {RequestUri}", requestUri);
                throw;
            }
        }
    }
}
