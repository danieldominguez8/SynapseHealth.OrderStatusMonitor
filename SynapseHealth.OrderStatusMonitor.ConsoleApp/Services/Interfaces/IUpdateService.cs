using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces
{
    public interface IUpdateService
    {
        Task UpdateMedicalEquipmentOrderAsync(JObject medicalEquipmentOrder);
    }
}
