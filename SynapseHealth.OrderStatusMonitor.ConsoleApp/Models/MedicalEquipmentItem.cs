using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Models
{
    public class MedicalEquipmentItem
    {
        public string Description { get; set; } 
        public string Status { get; set; } 
        public int DeliveryNotification { get; set; }
    }
}
