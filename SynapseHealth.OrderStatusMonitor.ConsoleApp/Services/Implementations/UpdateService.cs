using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations
{
    public class UpdateService : IUpdateService
    {
        public Task UpdateMedicalEquipmentOrderAsync(JObject medicalEquipmentOrder)
        {
            throw new NotImplementedException();
        }
    }
}
