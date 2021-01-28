#region using

using System;
using System.IO;
using System.Reflection;
using log4net;
using NetAppCommon.AppSettings.Models.Base;
using NetAppCommon.Helpers.Cache;
// ReSharper disable All

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
        ///Important !!!

        #region public AppSettingsModel()

        public AppSettingsModel()
        {
            try
            {
                var memoryCacheProvider = MemoryCacheProvider.GetInstance();
                var filePath = memoryCacheProvider.Get("NetAppCommon.AppSettings.Models.FilePath");
                if (null == filePath)
                {
                    var pathAppsettingsSetup = Path.Combine(BaseDirectory, "appsettings.setup.json");
                    var pathAppsettingsBase = Path.Combine(BaseDirectory, FileName);
                    var pathAppsettingsUserProfile = Path.Combine(UserProfileDirectory, FileName);
                    if (
                        File.Exists(pathAppsettingsSetup) && File.Exists(pathAppsettingsBase) && File.Exists(pathAppsettingsUserProfile)
                    )
                    {
                        if (File.GetLastWriteTime(pathAppsettingsSetup) >
                            File.GetLastWriteTime(pathAppsettingsUserProfile))
                        {
                            try
                            {
                                var appsettingsSetup = new AppSettingsModel(pathAppsettingsSetup);
                                var appsettingsBase = new AppSettingsModel(pathAppsettingsBase);
                                var appsettingsUserProfile = new AppSettingsModel(pathAppsettingsUserProfile);
                                appsettingsBase.ConnectionString = appsettingsSetup.ConnectionString ??
                                                                   appsettingsBase.ConnectionString;
                                appsettingsBase.AppSettingsRepository.Save(appsettingsBase);
                                appsettingsUserProfile.ConnectionString = appsettingsSetup.ConnectionString ??
                                                                          appsettingsUserProfile.ConnectionString;
                                appsettingsUserProfile.AppSettingsRepository.Save(appsettingsUserProfile);
#if DEBUG
                                _log4Net.Debug(
                                    $"NetAppCommon.AppSettings.Models appsettingsSetup {pathAppsettingsSetup} {File.GetLastWriteTime(pathAppsettingsSetup)} {appsettingsSetup.ConnectionString}");
                                _log4Net.Debug(
                                    $"NetAppCommon.AppSettings.Models appsettingsBase {pathAppsettingsBase} {File.GetLastWriteTime(pathAppsettingsBase)} {appsettingsBase.ConnectionString}");
                                _log4Net.Debug(
                                    $"NetAppCommon.AppSettings.Models appsettingsUserProfile {pathAppsettingsUserProfile} {File.GetLastWriteTime(pathAppsettingsUserProfile)} {appsettingsUserProfile.ConnectionString}");
#endif
                            }
                            catch (Exception e)
                            {
                                _log4Net.Error(
                                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                            }
                        }
                    }

                    AppSettingsRepository.MergeAndCopyToUserDirectory(this);
                    memoryCacheProvider.Put("NetAppCommon.AppSettings.Models.FilePath", FilePath, TimeSpan.FromDays(1));
                }

                FilePath = (string)(filePath ?? Path.Combine(UserProfileDirectory, FileName));
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }
        }

        #endregion

        ///Important !!!

        #region public static new AppSettingsModel GetInstance()

        /// <summary>
        ///     Pobierz statyczną referencję do instancji AppSettingsBaseModel
        ///     Get a static reference to the AppSettingsBaseModel instance
        /// </summary>
        /// <returns>
        ///     Statyczna referencja do instancji AppSettingsBaseModel
        ///     A static reference to the AppSettingsBaseModel instance
        /// </returns>
        public static AppSettingsModel GetInstance() => new();

        #endregion

        ///Important !!!

        #region public AppSettingsModel(string filePath)

        public AppSettingsModel(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileName = Path.GetFileName(filePath);
                FilePath = filePath;
            }
        }

        #endregion

        ///Important !!!

        #region public static new AppSettingsModel GetInstance()

        /// <summary>
        ///     Pobierz statyczną referencję do instancji AppSettingsBaseModel
        ///     Get a static reference to the AppSettingsBaseModel instance
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

        public sealed override string FileName
        {
            get => _fileName;
            protected set
            {
                if (value != _fileName)
                {
                    _fileName = value;
                    OnPropertyChanged("FileName");
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
                    OnPropertyChanged("ConnectionStringName");
                }
            }
        }

        #endregion
    }

    #endregion
}
