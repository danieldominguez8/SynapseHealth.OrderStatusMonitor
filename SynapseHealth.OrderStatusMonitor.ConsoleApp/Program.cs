using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
             .CreateLogger();

        try
        {
            // Create a service collection
            var serviceCollection = new ServiceCollection();

            // Register services
            serviceCollection.AddSingleton<ILogger>(Log.Logger);
            serviceCollection.AddSingleton<HttpClient>();
            serviceCollection.AddSingleton<IHttpClientService, HttpClientService>();
            serviceCollection.AddSingleton<IAlertService, AlertService>();
            serviceCollection.AddSingleton<IUpdateService, UpdateService>();
            serviceCollection.AddSingleton<IOrderService, OrderService>();

            // Build the service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Resolve the OrderService
            var orderService = serviceProvider.GetRequiredService<IOrderService>();

            // Fetch and process medical equipment orders
            var orders = await orderService.FetchMedicalEquipmentOrdersAsync();
            foreach (var order in orders)
            {
                await orderService.ProcessMedicalEquipmentOrderAsync(order);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled exception occurred during application execution");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
