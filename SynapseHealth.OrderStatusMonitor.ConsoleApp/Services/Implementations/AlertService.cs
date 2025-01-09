using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Implementations
{
    public class AlertService : IAlertService
    {
        public Task SendAlertAsync(string message)
        {
            throw new NotImplementedException();
        }
    }
}
