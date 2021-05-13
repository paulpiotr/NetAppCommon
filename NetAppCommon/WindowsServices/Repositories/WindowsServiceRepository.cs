#region using

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using log4net;
using NetAppCommon.WindowsServices.Models;
using NetAppCommon.WindowsServices.Repositories.Interface;

#endregion

namespace NetAppCommon.WindowsServices.Repositories
{
    public class WindowsServiceRepository : IWindowsServiceRepository
    {
        /// <summary>
        ///     Instancja do klasy Log4netLogger
        ///     Instance to Log4netLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        ///     Pobierz dane o usłudze windows według nazwy serwisu lub/i nazwy wyświetlanej serwisu
        ///     Get windows service data by service name and / or service display name
        /// </summary>
        /// <param name="serviceName">
        ///     Nazwa serwisu jako string
        ///     Service name as a string
        /// </param>
        /// <param name="serviceDisplayName">
        ///     Opcjonalnie wyświetlana nazwa serwisu jako string
        ///     Optionally display the servis name as a string
        /// </param>
        /// <returns>
        ///     Dane o usłudze jako WindowsServiceModel lub null
        ///     Service data as WindowsServiceModel or null
        /// </returns>
        public WindowsServiceModel GetWindowsService(string serviceName, string serviceDisplayName = null)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !string.IsNullOrWhiteSpace(serviceName))
                {
                    ServiceController serviceController = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName.Contains(serviceName) ||
                        x.DisplayName.Contains(serviceDisplayName ?? serviceName));
                    if (null != serviceController)
                    {
                        Process process = Process.GetProcesses().FirstOrDefault(w => w.ProcessName.Contains(serviceController.ServiceName) ||
                            w.ProcessName.Contains(serviceController.DisplayName));
                        if (null != process)
                        {
                            return new WindowsServiceModel(serviceController, process);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return null;
        }

        /// <summary>
        ///     Pobierz dane o usłudze windows według nazwy serwisu lub/i nazwy wyświetlanej serwisu asynchronicznie
        ///     Get windows service data by service name and / or service display name asynchronously
        /// </summary>
        /// <param name="serviceName">
        ///     Nazwa serwisu jako string
        ///     Service name as a string
        /// </param>
        /// <param name="serviceDisplayName">
        ///     Opcjonalnie wyświetlana nazwa serwisu jako string
        ///     Optionally display the servis name as a string
        /// </param>
        /// <returns>
        ///     Dane o usłudze jako WindowsServiceModel lub null
        ///     Service data as WindowsServiceModel or null
        /// </returns>
        public async Task<WindowsServiceModel> GetWindowsServiceAsync(string serviceName,
            string serviceDisplayName = null) =>
            await Task.Run(() => GetWindowsService(serviceName, serviceDisplayName));

        /// <summary>
        ///     Czy czas rozpoczęcia bieżącego procesu jest większy niż czas rozpoczęcia procesu usługi
        ///     Whether the current process's start time is greater than the service process's start time
        /// </summary>
        /// <param name="currentProcess">
        ///     Bieżący proces jako Process
        ///     Current process as Process
        /// </param>
        /// <param name="serviceName">
        ///     Nazwa serwisu jako string
        ///     Service name as a string
        /// </param>
        /// <param name="serviceDisplayName">
        ///     Opcjonalnie wyświetlana nazwa serwisu jako string
        ///     Optionally display the servis name as a string
        /// </param>
        /// <returns>
        ///     bool
        ///     bool
        /// </returns>
        public bool IsWhetherStartTimeCurrentProcessIsGreaterAsStartTimeServiceProcess(Process currentProcess,
            string serviceName, string serviceDisplayName = null)
        {
            try
            {
                WindowsServiceModel windowsServiceModel = GetWindowsService(serviceName, serviceDisplayName);
                if (null != currentProcess && null != windowsServiceModel)
                {
//#if DEBUG
//                    Console.WriteLine($"currentProcess {currentProcess.StartTime} windowsServiceModel.Process.StartTime {windowsServiceModel.Process?.StartTime} ");
//#endif
                    return currentProcess.StartTime > windowsServiceModel.Process?.StartTime;
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return false;
        }

        /// <summary>
        ///     Czy czas rozpoczęcia bieżącego procesu jest większy niż czas rozpoczęcia procesu usługi asynchronicznie
        ///     Whether the current process's start time is greater than the service process's start time asynchronously
        /// </summary>
        /// <param name="currentProcess">
        ///     Bieżący proces jako Process
        ///     Current process as Process
        /// </param>
        /// <param name="serviceName">
        ///     Nazwa serwisu jako string
        ///     Service name as a string
        /// </param>
        /// <param name="serviceDisplayName">
        ///     Opcjonalnie wyświetlana nazwa serwisu jako string
        ///     Optionally display the servis name as a string
        /// </param>
        /// <returns>
        ///     bool
        ///     bool
        /// </returns>
        public async Task<bool> IsWhetherStartTimeCurrentProcessIsGreaterAsStartTimeServiceProcessAsync(
            Process currentProcess, string serviceName, string serviceDisplayName = null) =>
            await Task.Run(() => IsWhetherStartTimeCurrentProcessIsGreaterAsStartTimeServiceProcess(currentProcess,
                serviceName, serviceDisplayName));

        /// <summary>
        ///     Czy czas rozpoczęcia bieżącego procesu jest mniejszy niż czas rozpoczęcia procesu usługi
        ///     Whether the current process's start time is less than the service's process start time
        /// </summary>
        /// <param name="currentProcess">
        ///     Bieżący proces jako Process
        ///     Current process as Process
        /// </param>
        /// <param name="serviceName">
        ///     Nazwa serwisu jako string
        ///     Service name as a string
        /// </param>
        /// <param name="serviceDisplayName">
        ///     Opcjonalnie wyświetlana nazwa serwisu jako string
        ///     Optionally display the servis name as a string
        /// </param>
        /// <returns>
        ///     bool
        ///     bool
        /// </returns>
        public bool IsWhetherStartTimeCurrentProcessIsLessAsStartTimeServiceProcess(Process currentProcess,
            string serviceName, string serviceDisplayName = null) =>
            !IsWhetherStartTimeCurrentProcessIsGreaterAsStartTimeServiceProcess(currentProcess, serviceName,
                serviceDisplayName);

        /// <summary>
        ///     Czy czas rozpoczęcia bieżącego procesu jest mniejszy niż czas rozpoczęcia procesu usługi
        ///     Whether the current process's start time is less than the service's process start time
        /// </summary>
        /// <param name="currentProcess">
        ///     Bieżący proces jako Process
        ///     Current process as Process
        /// </param>
        /// <param name="serviceName">
        ///     Nazwa serwisu jako string
        ///     Service name as a string
        /// </param>
        /// <param name="serviceDisplayName">
        ///     Opcjonalnie wyświetlana nazwa serwisu jako string
        ///     Optionally display the servis name as a string
        /// </param>
        /// <returns>
        ///     bool
        ///     bool
        /// </returns>
        public async Task<bool> IsWhetherStartTimeCurrentProcessIsLessAsStartTimeServiceProcessAsync(
            Process currentProcess, string serviceName, string serviceDisplayName = null) =>
            await Task.Run(() => IsWhetherStartTimeCurrentProcessIsLessAsStartTimeServiceProcess(currentProcess, serviceName,
                serviceDisplayName));
    }
}
