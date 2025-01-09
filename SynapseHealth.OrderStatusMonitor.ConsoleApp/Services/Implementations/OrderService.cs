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
        public Task<JObject[]> FetchMedicalEquipmentOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task ProcessMedicalEquipmentOrderAsync(JObject medicalEquipmentOrder)
        {
            throw new NotImplementedException();
        }
    }
}
