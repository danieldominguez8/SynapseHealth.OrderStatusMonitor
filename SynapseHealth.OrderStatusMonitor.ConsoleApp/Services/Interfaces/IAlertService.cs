using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces
{
    public interface IAlertService
    {
        Task SendAlertAsync(string message);
    }
}
