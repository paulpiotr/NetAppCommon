#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Xml.Serialization;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetAppCommon.AppSettings.Repositories.Base;
using NetAppCommon.Crypto.AesCryptography.Services;
using NetAppCommon.Crypto.BouncyCastle.Models.Base;
using NetAppCommon.Crypto.BouncyCastle.Services;
using NetAppCommon.Validation;
using Newtonsoft.Json;
using static System.Environment;

#endregion

#nullable enable annotations

namespace NetAppCommon.WindowsServices.Models
{
    #region public class WindowsServiceModel : INotifyPropertyChanged

    /// <summary>
    /// Klasa modelu zarządzania procesami (usługami) w systemie Windows
    /// Class of Windows process (service) management model
    /// </summary>
    public class WindowsServiceModel : INotifyPropertyChanged
    {
        #region public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     PropertyChangedEventHandler PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)

        /// <summary>
        ///     protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        #endregion

        #region protected void OnPropertyChanged(string propertyName)

        /// <summary>
        ///     protected void OnPropertyChanged(string propertyName)
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        #endregion

        #region public WindowsServiceModel(ServiceController serviceController, Process process)

        /// <summary>
        /// Konstruktor
        /// Constructor
        /// </summary>
        /// <param name="serviceController">
        /// Wymagany kontroler serwisu (usługi) windows jako ServiceController
        /// Required windows service controller as ServiceController
        /// </param>
        /// <param name="process">
        /// Wymagany proces główny serwisu (usługi) jako Process
        /// Required service main process as Process
        /// </param>
        public WindowsServiceModel(ServiceController serviceController, Process process)
        {
            _serviceController = serviceController;
            _process = process;
        }

        #endregion

        #region private ServiceController? _serviceController public ServiceController? ServiceController

        private ServiceController? _serviceController;

        /// <summary>
        /// Wymagany kontroler serwisu (usługi) windows jako ServiceController
        /// Required windows service controller as ServiceController
        /// </summary>
        public ServiceController? ServiceController
        {
            get => _serviceController;
            private set
            {
                if (value != _serviceController)
                {
                    _serviceController = value;
                    OnPropertyChanged(nameof(ServiceController));
                }
            }
        }

        #endregion

        #region private Process? _process; public Process? Process

        private Process? _process;

        /// <summary>
        /// Wymagany proces główny serwisu (usługi) jako Process
        /// Required service main process as Process
        /// </summary>
        public Process? Process
        {
            get => _process;
            private set
            {
                if (value != _process)
                {
                    _process = value;
                    OnPropertyChanged(nameof(Process));
                }
            }
        }

        #endregion
    }

    #endregion
}