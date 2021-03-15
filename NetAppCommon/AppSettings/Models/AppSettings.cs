#region using

using System;
using System.IO;
using System.Reflection;
using log4net;
using NetAppCommon.AppSettings.Models.Base;
using NetAppCommon.Helpers.Cache;

#endregion

#nullable enable annotations

namespace NetAppCommon.AppSettings.Models
{
    #region public class AppSettings

    /// <summary>
    ///     Model danych ustawień aplikacji NetAppCommon.AppSettings.Models
    ///     Data model for the NetAppCommon.AppSettings.Models application settings
    /// </summary>
    public sealed class AppSettings : AppSettingsWithDatabase
    {
        #region public AppSettingsModel()

        /// <summary>
        ///     Ważne - konstruktor
        ///     Important - the constructor
        /// </summary>
        public AppSettings()
        {
            try
            {
                var memoryCacheProvider = MemoryCacheProvider.GetInstance();
                var filePathKey = $"{MethodBase.GetCurrentMethod()?.DeclaringType?.FullName}.FilePath";
                var filePath = (object) memoryCacheProvider.Get(filePathKey);
                if (null == filePath)
                {
                    var appSettingsSetupFilePath = Path.Combine(BaseDirectory!, SetupFileName!);
                    var appSettingsUserFilePath = Path.Combine(UserProfileDirectory!, FileName!);
                    var appSettingsFilePath = FilePath;
                    var appSettingsSetupConnectionString =
                        new AppSettings(appSettingsSetupFilePath!).GetConnectionString();
                    var appSettingsUserConnectionString =
                        new AppSettings(appSettingsUserFilePath!).GetConnectionString();
                    var appSettingsConnectionString =
                        new AppSettings(appSettingsFilePath!).GetConnectionString();

                    if (!string.IsNullOrWhiteSpace(appSettingsUserFilePath) && !File.Exists(appSettingsUserFilePath))
                    {
                        AppSettingsRepository?.CopyToUserDirectory(this);
                    }

                    try
                    {
                        AppSettingsRepository?.MergeAndSave(appSettingsSetupFilePath, appSettingsUserFilePath);
                    }
                    catch (Exception e)
                    {
                        _log4Net.Error($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n",
                            e);
                    }

                    try
                    {
                        if (File.Exists(appSettingsSetupFilePath))
                        {
                            File.Delete(appSettingsSetupFilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        _log4Net.Error($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n",
                            e);
                    }

                    try
                    {
                        AppSettingsRepository?.MergeAndSave(appSettingsUserFilePath, appSettingsFilePath);
                    }
                    catch (Exception e)
                    {
                        _log4Net.Error($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n",
                            e);
                    }

                    try
                    {
                        AppSettingsRepository?.MergeAndSave(appSettingsFilePath, appSettingsUserFilePath);
                    }
                    catch (Exception e)
                    {
                        _log4Net.Error($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n",
                            e);
                    }

                    FilePath = File.Exists(appSettingsUserFilePath) ? appSettingsUserFilePath : FilePath;

                    if (null != AppSettingsRepository)
                    {
                        if (!string.IsNullOrWhiteSpace(appSettingsSetupConnectionString) &&
                            AppSettingsRepository.MssqlCheckConnectionString(appSettingsSetupConnectionString))
                        {
                            ConnectionString = appSettingsSetupConnectionString;
                            AppSettingsRepository?.SaveAsync(this);
                        }
                        else if (!string.IsNullOrWhiteSpace(appSettingsUserConnectionString) &&
                                 AppSettingsRepository.MssqlCheckConnectionString(appSettingsUserConnectionString))
                        {
                            ConnectionString = appSettingsUserConnectionString;
                            AppSettingsRepository?.SaveAsync(this);
                        }
                        else if (!string.IsNullOrWhiteSpace(appSettingsConnectionString) &&
                                 AppSettingsRepository.MssqlCheckConnectionString(appSettingsConnectionString))
                        {
                            ConnectionString = appSettingsConnectionString;
                            AppSettingsRepository?.SaveAsync(this);
                        }
                    }

                    memoryCacheProvider.Put(filePathKey, FilePath, TimeSpan.FromDays(1));
                }

                if (null != UserProfileDirectory && null != FileName)
                {
                    FilePath = (string) (filePath ?? Path.Combine(UserProfileDirectory!, FileName!));
                }
            }
            catch (Exception e)
            {
                _log4Net.Error($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }
        }

        #endregion

        #region public static new AppSettingsModel GetInstance()

        /// <summary>
        ///     Ważne - Pobierz statyczną referencję do instancji AppSettingsBaseModel
        ///     Important - Get a static reference to the AppSettingsBaseModel instance
        /// </summary>
        /// <returns>
        ///     Statyczna referencja do instancji AppSettingsBaseModel
        ///     A static reference to the AppSettingsBaseModel instance
        /// </returns>
        public static AppSettings GetAppSettingsModel()
        {
            return new();
        }

        #endregion

        #region public AppSettingsModel(string filePath)

        /// <summary>
        ///     Ważne - konstruktor
        ///     Important - the constructor
        /// </summary>
        /// <param name="filePath">
        ///     Ścieżka do pliku konfiguracji jako string
        ///     Path to the configuration file as a string
        /// </param>
        public AppSettings(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileName = Path.GetFileName(filePath);
                FilePath = filePath;
            }
        }

        #endregion

        #region public static new AppSettingsModel GetInstance()

        /// <summary>
        ///     Ważne - Pobierz statyczną referencję do instancji AppSettingsBaseModel
        ///     Important - Get a static reference to the AppSettingsBaseModel instance
        /// </summary>
        /// <returns>
        ///     Statyczna referencja do instancji AppSettingsBaseModel
        ///     A static reference to the AppSettingsBaseModel instance
        /// </returns>
        public static AppSettings GetAppSettingsModel(string filePath)
        {
            return new(filePath);
        }

        #endregion

        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Instancja do klasy Log4netLogger
        ///     Instance to Log4netLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region protected new string _fileName public override string FileName

#if DEBUG
        private const string Filename = "appsettings.json";
#else
        private const string Filename = "appsettings.json";
#endif

        private string? _fileName = Filename;

        public override string? FileName
        {
            get => _fileName;
            protected set
            {
                if (value != _fileName)
                {
                    _fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        #endregion

        #region private new string? _setupFileName = Setupfilename; public override string? SetupFileName

#if DEBUG
        private const string Setupfilename = "appsettings.setup.json";
#else
        private const string Setupfilename = "appsettings.setup.json";
#endif

        private string? _setupFileName = Setupfilename;

        public override string? SetupFileName
        {
            get => _setupFileName;
            protected set
            {
                if (value != _setupFileName)
                {
                    _setupFileName = value;
                    OnPropertyChanged("SetupFileName");
                }
            }
        }

        #endregion

        #region protected new string _connectionStringName public override string ConnectionStringName

#if DEBUG
        private const string Connectionstringname = "DefaultDatabaseContext";
#else
        private const string Connectionstringname = "DefaultDatabaseContext";
#endif

        private string _connectionStringName = Connectionstringname;

        public override string ConnectionStringName
        {
            get => _connectionStringName;
            set
            {
                if (value != _connectionStringName)
                {
                    _connectionStringName = value;
                    OnPropertyChanged(nameof(ConnectionStringName));
                }
            }
        }

        #endregion

        #region private string _lastInstallDate; public string LastInstallDate

        private string? _lastInstallDate;

        public string? LastInstallDate
        {
            get
            {
                if (null == _lastInstallDate)
                {
                    _lastInstallDate = AppSettingsRepository?.GetValue<string>(this, nameof(LastInstallDate));
                }

                return _lastInstallDate;
            }
            set
            {
                if (value != _lastInstallDate)
                {
                    _lastInstallDate = value;
                    OnPropertyChanged(nameof(LastInstallDate));
                }
            }
        }

        #endregion

        #region private string _productCode; public string ProductCode

        private string? _productCode;

        public string? ProductCode
        {
            get
            {
                if (null == _productCode)
                {
                    _productCode = AppSettingsRepository?.GetValue<string>(this,
                        nameof(ProductCode));
                }

                return _productCode;
            }
            set
            {
                if (value != _productCode)
                {
                    _productCode = value;
                    OnPropertyChanged(nameof(ProductCode));
                }
            }
        }

        #endregion

        #region private string _productVersion; public string ProductVersion

        private string? _productVersion;

        public string? ProductVersion
        {
            get
            {
                if (null == _productVersion)
                {
                    _productVersion = AppSettingsRepository?.GetValue<string>(this,
                        nameof(ProductVersion));
                }

                return _productVersion;
            }
            set
            {
                if (value != _productVersion)
                {
                    _productVersion = value;
                    OnPropertyChanged(nameof(ProductVersion));
                }
            }
        }

        #endregion

        #region private string _upgradeCode; public string UpgradeCode

        private string? _upgradeCode;

        public string? UpgradeCode
        {
            get
            {
                if (null == _upgradeCode)
                {
                    _upgradeCode = AppSettingsRepository?.GetValue<string>(this,
                        nameof(UpgradeCode));
                }

                return _upgradeCode;
            }
            set
            {
                if (value != _upgradeCode)
                {
                    _upgradeCode = value;
                    OnPropertyChanged(nameof(UpgradeCode));
                }
            }
        }

        #endregion
    }

    #endregion
}
