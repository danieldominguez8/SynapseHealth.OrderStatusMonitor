using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Models;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces
{
    internal interface IOrderService
    {
        Task<List<MedicalEquipmentOrder>> FetchMedicalEquipmentOrdersAsync();
        Task ProcessMedicalEquipmentOrderAsync(MedicalEquipmentOrder order);
    }
}
