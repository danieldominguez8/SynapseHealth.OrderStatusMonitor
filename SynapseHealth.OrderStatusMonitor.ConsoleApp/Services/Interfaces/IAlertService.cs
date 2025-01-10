using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SynapseHealth.OrderStatusMonitor.ConsoleApp.Models;

namespace SynapseHealth.OrderStatusMonitor.ConsoleApp.Services.Interfaces
{
    public interface IAlertService
    {
        Task SendAlertAsync(Alert alert);
    }
}
