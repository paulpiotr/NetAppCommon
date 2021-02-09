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
    public sealed class AppSettingsModel : AppSettingsBaseModel
    {
        #region public AppSettingsModel()

        /// <summary>
        ///     Ważne - konstruktor
        ///     Important - the constructor
        /// </summary>
        public AppSettingsModel()
        {
            try
            {
                if (null != BaseDirectory && null != UserProfileDirectory)
                {
                    var memoryCacheProvider = MemoryCacheProvider.GetInstance();
                    object filePath = memoryCacheProvider.Get("NetAppCommon.AppSettings.Models.FilePath");
                    if (null == filePath)
                    {
                        var pathAppSettingsSetup = Path.Combine(BaseDirectory, "appsettings.setup.json");
                        var pathAppSettingsBase = Path.Combine(BaseDirectory, FileName);
                        var pathAppSettingsBaseUser = Path.Combine(UserProfileDirectory, FileName);
                        if (
                            File.Exists(pathAppSettingsSetup)
                            &&
                            File.Exists(pathAppSettingsBase)
                            //&&
                            //File.Exists(pathAppSettingsBaseUser)
                        )
                        {
                            if (File.GetLastWriteTime(pathAppSettingsSetup) >
                                File.GetLastWriteTime(pathAppSettingsBaseUser))
                            {
                                try
                                {
                                    var appsettingsSetup = new AppSettingsModel(pathAppSettingsSetup);
                                    var appsettingsBase = new AppSettingsModel(pathAppSettingsBase);
                                    var appsettingsBaseUser = new AppSettingsModel(pathAppSettingsBaseUser);

                                    appsettingsBase.ConnectionString = appsettingsSetup.ConnectionString ??
                                                                       appsettingsBase.ConnectionString;

                                    appsettingsBase.LastInstallDate =
                                        appsettingsSetup.AppSettingsRepository?.GetValue<string>(appsettingsSetup,
                                            nameof(LastInstallDate));

                                    appsettingsBase.ProductCode =
                                        appsettingsSetup.AppSettingsRepository?.GetValue<string>(appsettingsSetup,
                                            nameof(ProductCode));

                                    appsettingsBase.ProductVersion =
                                        appsettingsSetup.AppSettingsRepository?.GetValue<string>(appsettingsSetup,
                                            nameof(ProductVersion));

                                    appsettingsBase.UpgradeCode =
                                        appsettingsSetup.AppSettingsRepository?.GetValue<string>(appsettingsSetup,
                                            nameof(UpgradeCode));

                                    //appsettingsBaseappsettingsBase.AppSettingsRepository?.Save(appsettingsBase);
                                    appsettingsBase.AppSettingsRepository?.MergeAndSave(appsettingsBase);

                                    appsettingsBaseUser.ConnectionString = appsettingsSetup.ConnectionString ??
                                                                           appsettingsBaseUser.ConnectionString;

                                    appsettingsBaseUser.LastInstallDate =
                                        appsettingsSetup.AppSettingsRepository?.GetValue<string>(appsettingsSetup,
                                            nameof(LastInstallDate));

                                    appsettingsBaseUser.ProductCode =
                                        appsettingsSetup.AppSettingsRepository?.GetValue<string>(appsettingsSetup,
                                            nameof(ProductCode));

                                    appsettingsBaseUser.ProductVersion =
                                        appsettingsSetup.AppSettingsRepository?.GetValue<string>(appsettingsSetup,
                                            nameof(ProductVersion));

                                    appsettingsBaseUser.UpgradeCode =
                                        appsettingsSetup.AppSettingsRepository?.GetValue<string>(appsettingsSetup,
                                            nameof(UpgradeCode));

                                    //appsettingsBaseUser.AppSettingsRepository?.Save(appsettingsBaseUser);
                                    appsettingsBaseUser.AppSettingsRepository?.MergeAndSave(appsettingsBaseUser);
#if DEBUG
                                    _log4Net.Debug(
                                        $"NetAppCommon.AppSettings.Models appsettingsSetup {pathAppSettingsSetup} {File.GetLastWriteTime(pathAppSettingsSetup)} {appsettingsSetup.ConnectionString}");
                                    _log4Net.Debug(
                                        $"NetAppCommon.AppSettings.Models appsettingsBase {pathAppSettingsBase} {File.GetLastWriteTime(pathAppSettingsBase)} {appsettingsBase.ConnectionString}");
                                    _log4Net.Debug(
                                        $"NetAppCommon.AppSettings.Models appsettingsBaseUser {pathAppSettingsBaseUser} {File.GetLastWriteTime(pathAppSettingsBaseUser)} {appsettingsBaseUser.ConnectionString}");
#endif
                                    File.Delete(pathAppSettingsSetup);
                                }
                                catch (Exception e)
                                {
                                    _log4Net.Error(
                                        $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n",
                                        e);
                                }
                            }
                        }

                        AppSettingsRepository?.MergeAndCopyToUserDirectory(this);
                        memoryCacheProvider.Put("NetAppCommon.AppSettings.Models.FilePath", FilePath,
                            TimeSpan.FromDays(1));
                    }

                    FilePath = (string)(filePath ?? Path.Combine(UserProfileDirectory, FileName));
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
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
        public static AppSettingsModel GetInstance() => new();

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
        public AppSettingsModel(string filePath)
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
        public static AppSettingsModel GetInstance(string filePath) => new(filePath);

        #endregion

        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Instancja do klasy Log4netLogger
        ///     Instance to Log4netLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region protected new string _fileName public override string FileName

#if DEBUG
        private const string Filename = "appsettings.json";
#else
        private const string Filename = "appsettings.json";
#endif

        private new string _fileName = Filename;

        public override string FileName
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

        #region protected new string _connectionStringName public override string ConnectionStringName

#if DEBUG
        private const string Connectionstringname = "DefaultDatabaseContext";
#else
        private const string Connectionstringname = "DefaultDatabaseContext";
#endif

        private new string _connectionStringName = Connectionstringname;

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
