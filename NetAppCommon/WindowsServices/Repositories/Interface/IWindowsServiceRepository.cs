using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using NetAppCommon.WindowsServices.Models;

namespace NetAppCommon.WindowsServices.Repositories.Interface
{
    public interface IWindowsServiceRepository
    {
        public WindowsServiceModel GetWindowsService(string serviceName, string serviceDisplayName = null);

        public Task<WindowsServiceModel> GetWindowsServiceAsync(string serviceName, string serviceDisplayName = null);

        public bool IsWhetherStartTimeCurrentProcessIsGreaterAsStartTimeServiceProcess(Process currentProcess, string serviceName, string serviceDisplayName = null);

        public Task<bool> IsWhetherStartTimeCurrentProcessIsGreaterAsStartTimeServiceProcessAsync(Process currentProcess, string serviceName, string serviceDisplayName = null);

        public bool IsWhetherStartTimeCurrentProcessIsLessAsStartTimeServiceProcess(Process currentProcess, string serviceName, string serviceDisplayName = null);

        public Task<bool> IsWhetherStartTimeCurrentProcessIsLessAsStartTimeServiceProcessAsync(Process currentProcess, string serviceName, string serviceDisplayName = null);
    }
}
