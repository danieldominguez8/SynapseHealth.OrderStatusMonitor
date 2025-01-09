using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Models
{
    public class MedicalEquipmentOrder
    {
        public string OrderId { get; set; } 
        public List<MedicalEquipmentItem> MedicalEquipmentItems { get; set; } 
    }
}
