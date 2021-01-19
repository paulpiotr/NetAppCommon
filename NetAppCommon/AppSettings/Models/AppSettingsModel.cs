using System;
using System.IO;
using System.Reflection;
using NetAppCommon.AppSettings.Models.Base;

#nullable enable annotations

namespace NetAppCommon.AppSettings.Models
{
    #region public class AppSettings
    /// <summary>
    /// Model danych ustawień aplikacji NetAppCommon.AppSettings.Models
    /// Data model for the NetAppCommon.AppSettings.Models application settings
    /// </summary>
    public class AppSettingsModel : AppSettingsBaseModel
    {
        ///Important !!!
        #region public AppSettingsModel()
        public AppSettingsModel()
        {
            try
            {
                var memoryCacheProvider = Helpers.Cache.MemoryCacheProvider.GetInstance();
                var filePath = memoryCacheProvider.Get("NetAppCommon.AppSettings.Models.FilePath");
                if (null == filePath)
                {
                    var pathAppsettingsSetup = Path.Combine(BaseDirectory, "appsettings.setup.json");
                    var pathAppsettingsBase = Path.Combine(BaseDirectory, FileName);
                    var pathAppsettingsUserProfile = Path.Combine(UserProfileDirectory, FileName);
                    if (
                        null != pathAppsettingsSetup && File.Exists(pathAppsettingsSetup) &&
                        null != pathAppsettingsBase && File.Exists(pathAppsettingsBase) &&
                        null != pathAppsettingsUserProfile && File.Exists(pathAppsettingsUserProfile)
                        )
                    {
                        if (File.GetLastWriteTime(pathAppsettingsSetup) > File.GetLastWriteTime(pathAppsettingsUserProfile))
                        {
                            try
                            {
                                var appsettingsSetup = new AppSettingsModel(pathAppsettingsSetup);
                                var appsettingsBase = new AppSettingsModel(pathAppsettingsBase);
                                var appsettingsUserProfile = new AppSettingsModel(pathAppsettingsUserProfile);
                                appsettingsBase.ConnectionString = appsettingsSetup.ConnectionString ?? appsettingsBase.ConnectionString;
                                appsettingsBase.AppSettingsRepository.Save(appsettingsBase);
                                appsettingsUserProfile.ConnectionString = appsettingsSetup.ConnectionString ?? appsettingsUserProfile.ConnectionString;
                                appsettingsUserProfile.AppSettingsRepository.Save(appsettingsUserProfile);
#if DEBUG
                                _log4net.Debug($"NetAppCommon.AppSettings.Models appsettingsSetup { pathAppsettingsSetup } { File.GetLastWriteTime(pathAppsettingsSetup) } { appsettingsSetup.ConnectionString }");
                                _log4net.Debug($"NetAppCommon.AppSettings.Models appsettingsBase { pathAppsettingsBase } { File.GetLastWriteTime(pathAppsettingsBase) } { appsettingsBase.ConnectionString }");
                                _log4net.Debug($"NetAppCommon.AppSettings.Models appsettingsUserProfile { pathAppsettingsUserProfile } { File.GetLastWriteTime(pathAppsettingsUserProfile) } { appsettingsUserProfile.ConnectionString }");
#endif
                            }
                            catch (Exception e)
                            {
                                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
                            }
                        }
                    }
                    AppSettingsRepository.MergeAndCopyToUserDirectory(this);
                    memoryCacheProvider.Put("NetAppCommon.AppSettings.Models.FilePath", FilePath, TimeSpan.FromDays(1));
                }
                if (null != UserProfileDirectory && null != FileName)
                {
                    FilePath = (string)(filePath ?? Path.Combine(UserProfileDirectory, FileName));
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
        }
        #endregion

        ///Important !!!
        #region public static new AppSettingsModel GetInstance()
        /// <summary>
        /// Pobierz statyczną referencję do instancji AppSettingsBaseModel
        /// Get a static reference to the AppSettingsBaseModel instance
        /// </summary>
        /// <returns>
        /// Statyczna referencja do instancji AppSettingsBaseModel
        /// A static reference to the AppSettingsBaseModel instance
        /// </returns>
        public static AppSettingsModel GetInstance()
        {
            return new AppSettingsModel();
        }
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
        /// Pobierz statyczną referencję do instancji AppSettingsBaseModel
        /// Get a static reference to the AppSettingsBaseModel instance
        /// </summary>
        /// <returns>
        /// Statyczna referencja do instancji AppSettingsBaseModel
        /// A static reference to the AppSettingsBaseModel instance
        /// </returns>
        public static AppSettingsModel GetInstance(string filePath)
        {
            return new AppSettingsModel(filePath);
        }
        #endregion

        #region private readonly log4net.ILog log4net
        /// <summary>
        /// Instancja do klasy Log4netLogger
        /// Instance to Log4netLogger class
        /// </summary>
        private readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region protected new string _fileName public override string FileName

#if DEBUG
        private const string FILENAME = "appsettings.json";
#else
        private const string FILENAME = "appsettings.json";
#endif

        protected new string _fileName = FILENAME;

        public override string FileName
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
        private const string CONNECTIONSTRINGNAME = "DefaultDatabaseContext";
#else
        private const string CONNECTIONSTRINGNAME = "DefaultDatabaseContext";
#endif

        protected new string _connectionStringName = CONNECTIONSTRINGNAME;

        public override string ConnectionStringName
        {
            get => _connectionStringName;
            set
            {
                if (value != _connectionStringName)
                {
                    _connectionStringName = value;
                }
            }
        }
        #endregion
    }
    #endregion
}
