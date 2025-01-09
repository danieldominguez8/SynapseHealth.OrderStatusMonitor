using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces
{
    internal interface IOrderService
    {
        Task<JObject[]> FetchMedicalEquipmentOrdersAsync();
        Task ProcessMedicalEquipmentOrderAsync(JObject medicalEquipmentOrder);
    }
}
