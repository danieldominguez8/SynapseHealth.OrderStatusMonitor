using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces
{
    internal interface IOrderService
    {
        Task<JObject[]> FetchOrdersAsync();
        Task ProcessOrderAsync(JObject order);
    }
}
