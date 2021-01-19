using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetAppCommon.AppSettings.Models.Base;
using NetAppCommon.AppSettings.Repositories.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetAppCommon.AppSettings.Repositories
{
    #region public class AppSettingsRepositoryBase<TAppSettings> : IAppSettingsRepository<TAppSettings> where TAppSettings : AppSettingsBaseModel, new()
    /// <summary>
    /// Klasa bazowa repozytorium ustawień
    /// Base class for the settings repository
    /// </summary>
    /// <typeparam name="TAppSettings">
    /// Parametr typu modelu ustawień odziedziczony po AppSettingsBaseModel
    /// Settings model type parameter inherited from AppSettingsBaseModel
    /// </typeparam>
    public class AppSettingsRepositoryBase<TAppSettings> : IAppSettingsRepository<TAppSettings> where TAppSettings : AppSettingsBaseModel, new()
    {
        #region private readonly log4net.ILog _log4net
        /// <summary>
        /// Referencja klasy Log4NetLogget
        /// Reference to the Log4NetLogget class
        /// </summary>
        private readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public static AppSettingsRepositoryBase<TAppSettings> GetInstance()
        /// <summary>
        /// Pobierz statyczną referencję do klasy AppSettingsRepositoryBase
        /// Get a static reference to the AppSettingsRepositoryBase class
        /// </summary>
        /// <returns>
        /// Statyczna referencja do klasy AppSettingsRepositoryBase
        /// A static reference to the AppSettingsRepositoryBase class
        /// </returns>
        public static AppSettingsRepositoryBase<TAppSettings> GetInstance()
        {
            return new AppSettingsRepositoryBase<TAppSettings>();
        }
        #endregion

        #region public virtual TAppSettings MergeAndSave(TAppSettings appSettings = null)
        /// <summary>
        /// Połącz i zapisz ustawienia
        /// Connect and save the settings
        /// </summary>
        /// <param name="appSettings">
        /// Instancja do modelu ustawień TAppSettings : AppSettingsBaseModel, new()
        /// Instance to TAppSettings settings model: AppSettingsBaseModel, new ()
        /// </param>
        /// <returns>
        /// Nowy obiekt instancji ustawień jako TAppSettings : AppSettingsBaseModel, new()
        /// New settings instance object as TAppSettings: AppSettingsBaseModel, new ()
        /// </returns>
        public virtual TAppSettings MergeAndSave(TAppSettings appSettings = null)
        {
            try
            {
                appSettings ??= new TAppSettings();
                var filePath = appSettings.FilePath;
                if (null != filePath && !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    var originalObject = JObject.Parse(File.ReadAllText(filePath));
                    var appSettingsObject = JObject.Parse(JsonConvert.SerializeObject(appSettings, Formatting.Indented));
                    if (null != originalObject && null != appSettingsObject)
                    {
                        originalObject.Merge(appSettingsObject, new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        });
                        if (null != originalObject)
                        {
                            Helpers.Files.FileHelper.GetInstance().TimeoutAction(() =>
                            {
                                File.WriteAllText(filePath, originalObject.ToString());
                                return true;
                            }, filePath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return new TAppSettings();
        }
        #endregion

        #region public virtual async Task<TAppSettings> MergeAndSaveAsync(TAppSettings appSettings = null)
        /// <summary>
        /// Połącz i zapisz ustawienia synchronicznie
        /// Connect and save the settings synchronously
        /// </summary>
        /// <param name="appSettings">
        /// Instancja do modelu ustawień TAppSettings : AppSettingsBaseModel, new()
        /// Instance to TAppSettings settings model: AppSettingsBaseModel, new ()
        /// </param>
        /// <returns>
        /// Nowy obiekt instancji ustawień jako TAppSettings : AppSettingsBaseModel, new()
        /// New settings instance object as TAppSettings: AppSettingsBaseModel, new ()
        /// </returns>
        public virtual async Task<TAppSettings> MergeAndSaveAsync(TAppSettings appSettings = null)
        {
            return await Task.Run(() =>
            {
                return MergeAndSave(appSettings);
            });
        }
        #endregion

        #region public virtual TAppSettings Save(TAppSettings appSettings = null)
        /// <summary>
        /// Zapis ustawienia
        /// Save the setting
        /// </summary>
        /// <param name="appSettings">
        /// Instancja do modelu ustawień TAppSettings : AppSettingsBaseModel, new()
        /// Instance to TAppSettings settings model: AppSettingsBaseModel, new ()
        /// </param>
        /// <returns>
        /// Nowy obiekt instancji ustawień jako TAppSettings : AppSettingsBaseModel, new()
        /// New settings instance object as TAppSettings: AppSettingsBaseModel, new ()
        /// </returns>
        public virtual TAppSettings Save(TAppSettings appSettings = null)
        {
            try
            {
                appSettings ??= new TAppSettings();
                var filePath = appSettings.FilePath;
                if (null != filePath && !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    var json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
                    if (null != json)
                    {
                        Helpers.Files.FileHelper.GetInstance().TimeoutAction(() =>
                        {
                            File.WriteAllText(filePath, json);
                            return true;
                        }, filePath);
                    }
                    json = null;
                }
                filePath = null;
                appSettings = null;
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return new TAppSettings();
        }
        #endregion

        #region public virtual async Task<TAppSettings> SaveAsync(TAppSettings appSettings = null)
        /// <summary>
        /// Zapis ustawienia asynchronicznie
        /// Save the setting asynchronously
        /// </summary>
        /// <param name="appSettings">
        /// Instancja do modelu ustawień TAppSettings : AppSettingsBaseModel, new()
        /// Instance to TAppSettings settings model: AppSettingsBaseModel, new ()
        /// </param>
        /// <returns>
        /// Nowy obiekt instancji ustawień jako TAppSettings : AppSettingsBaseModel, new()
        /// New settings instance object as TAppSettings: AppSettingsBaseModel, new ()
        /// </returns>
        public virtual async Task<TAppSettings> SaveAsync(TAppSettings appSettings = null)
        {
            return await Task.Run(() =>
            {
                return Save(appSettings);
            });
        }
        #endregion

        #region public virtual TAppSettings CopyToUserDirectory(TAppSettings appSettings = null)
        /// <summary>
        /// Skopiuj plik ustawień do katalogu użytkownika
        /// Copy the settings file to your user directory
        /// </summary>
        /// <param name="appSettings">
        /// Instancja do modelu ustawień TAppSettings : AppSettingsBaseModel, new()
        /// Instance to TAppSettings settings model: AppSettingsBaseModel, new ()
        /// </param>
        /// <returns>
        /// Nowy obiekt instancji ustawień jako TAppSettings : AppSettingsBaseModel, new()
        /// New settings instance object as TAppSettings: AppSettingsBaseModel, new ()
        /// </returns>
        public virtual TAppSettings CopyToUserDirectory(TAppSettings appSettings = null)
        {
            try
            {
                appSettings ??= new TAppSettings();
                var fileName = appSettings.GetFileName();
                var sourceFileName = appSettings.GetFilePath();
                var destFileName = Path.Combine(appSettings.UserProfileDirectory, fileName);
                if (File.Exists(sourceFileName) && !File.Exists(destFileName))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(destFileName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
                    }
                    File.Copy(sourceFileName, destFileName);
                }
                if (File.Exists(destFileName))
                {
                    appSettings.FilePath = destFileName;
                }
                fileName = null;
                sourceFileName = null;
                destFileName = null;
                appSettings = null;
            }
            catch (Exception e)
            {
                _log4net.Warn(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return new TAppSettings();
        }
        #endregion

        #region public virtual async Task<TAppSettings> CopyToUserDirectoryAsync(TAppSettings appSettings = null)
        /// <summary>
        /// Skopiuj plik ustawień do katalogu użytkownika asynchronicznie
        /// Copy the settings file to your user directory asynchronously
        /// </summary>
        /// <param name="appSettings">
        /// Instancja do modelu ustawień TAppSettings : AppSettingsBaseModel, new()
        /// Instance to TAppSettings settings model: AppSettingsBaseModel, new ()
        /// </param>
        /// <returns>
        /// Nowy obiekt instancji ustawień jako TAppSettings : AppSettingsBaseModel, new()
        /// New settings instance object as TAppSettings: AppSettingsBaseModel, new ()
        /// </returns>
        public virtual async Task<TAppSettings> CopyToUserDirectoryAsync(TAppSettings appSettings = null)
        {
            return await Task.Run(() =>
            {
                return CopyToUserDirectory(appSettings);
            });
        }
        #endregion

        #region public virtual TAppSettings MergeAndCopyToUserDirectory(TAppSettings appSettings = null)
        /// <summary>
        /// Połącz i skasuj ustawienia do katalogu profilu użytkownika
        /// Connect and reset the settings to the user profile directory
        /// </summary>
        /// <param name="appSettings">
        /// Instancja do modelu ustawień TAppSettings : AppSettingsBaseModel, new()
        /// Instance to TAppSettings settings model: AppSettingsBaseModel, new ()
        /// </param>
        /// <returns>
        /// Nowy obiekt instancji ustawień jako TAppSettings : AppSettingsBaseModel, new()
        /// New settings instance object as TAppSettings: AppSettingsBaseModel, new ()
        /// </returns>
        public virtual TAppSettings MergeAndCopyToUserDirectory(TAppSettings appSettings = null)
        {
            try
            {
                appSettings ??= new TAppSettings();
                if (null != appSettings.GetFileName() && null != appSettings.GetFileName() && null != appSettings.UserProfileDirectory)
                {
                    var fileName = appSettings.GetFileName();
                    var sourceFileName = appSettings.GetFilePath();
                    var destFileName = Path.Combine(appSettings.UserProfileDirectory, fileName);
                    if (File.Exists(sourceFileName) /*&& !File.Exists(destFileName)*/ && Directory.Exists(appSettings.UserProfileDirectory))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(destFileName)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
                        }
                        if (!File.Exists(destFileName))
                        {
                            File.Copy(sourceFileName, destFileName);
                        }
                        appSettings.FilePath = destFileName;
                        appSettings = MergeAndSave(appSettings);
                    }
                    if (File.Exists(destFileName))
                    {
                        appSettings.FilePath = destFileName;
                    }
                    fileName = null;
                    sourceFileName = null;
                    destFileName = null;
                    //appSettings = null;
                }
            }
            catch (Exception e)
            {
                _log4net.Warn(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return appSettings ?? new TAppSettings();
        }
        #endregion

        #region public virtual Task<TAppSettings> MergeAndCopyToUserDirectoryAsync(TAppSettings appSettings = null)
        /// <summary>
        /// Połącz i skasuj ustawienia do katalogu profilu użytkownika asynchronicznie
        /// Connect and reset the settings to the user profile directory asynchronously
        /// </summary>
        /// <param name="appSettings">
        /// Instancja do modelu ustawień TAppSettings : AppSettingsBaseModel, new()
        /// Instance to TAppSettings settings model: AppSettingsBaseModel, new ()
        /// </param>
        /// <returns>
        /// Nowy obiekt instancji ustawień jako TAppSettings : AppSettingsBaseModel, new()
        /// New settings instance object as TAppSettings: AppSettingsBaseModel, new ()
        /// </returns>
        public virtual async Task<TAppSettings> MergeAndCopyToUserDirectoryAsync(TAppSettings appSettings = null)
        {
            return await Task.Run(() =>
            {
                return MergeAndCopyToUserDirectory(appSettings);
            });
        }
        #endregion

        #region private TValue GetDefaultValue<TValue>()
        /// <summary>
        /// Pobierz domyślną wartość określonego typu określonego w parametrze TValue
        /// Get the default value of the specified type specified in the TValue parameter
        /// </summary>
        /// <typeparam name="TValue">
        /// Typ zwracanej wartości
        /// The type of the return value
        /// </typeparam>
        /// <returns>
        /// Domyślna wartość określonego typu określonego w parametrze TValue
        /// The default value of the specified type specified in the TValue parameter
        /// </returns>
        private TValue GetDefaultValue<TValue>()
        {
            return typeof(TValue).FullName switch
            {
                "System.Boolean" => (TValue)Convert.ChangeType(false, typeof(TValue)),
                "System.Int32" => (TValue)Convert.ChangeType(int.MinValue, typeof(TValue)),
                "System.DateTime" => (TValue)Convert.ChangeType(DateTime.MinValue, typeof(TValue)),
                _ => (TValue)Convert.ChangeType(null, typeof(TValue)),
            };
        }
        #endregion

        #region public virtual TValue GetValue<TValue>(string key)
        /// <summary>
        /// Pobierz wartość ustawienia z pliku konfiguracji .json na podstawie klucza
        /// Get the setting value from the .json configuration file based on the key
        /// </summary>
        /// <typeparam name="TValue">
        /// Typ zwracanej wartości
        /// The type of the return value
        /// </typeparam>
        /// <param name="key">
        /// Klucz
        /// Key
        /// </param>
        /// <returns>
        /// Wartość ustawienia na podstawie klucza jako typ określony w TValue
        /// The key-based setting value as the type specified in TValue
        /// </returns>
        public virtual TValue GetValue<TValue>(string key)
        {
            return GetValue<TValue>(new TAppSettings(), key);
        }
        #endregion

        #region public virtual async Task<TValue> GetValueAsync<TValue>(string key)
        /// <summary>
        /// Pobierz wartość ustawienia z pliku konfiguracji .json na podstawie klucza asynchronicznie
        /// Get the setting value from the .json configuration file based on the key asynchronously
        /// </summary>
        /// <typeparam name="TValue">
        /// Typ zwracanej wartości
        /// The type of the return value
        /// </typeparam>
        /// <param name="key">
        /// Klucz
        /// Key
        /// </param>
        /// <returns>
        /// Wartość ustawienia na podstawie klucza jako typ określony w TValue
        /// The key-based setting value as the type specified in TValue
        /// </returns>
        public virtual async Task<TValue> GetValueAsync<TValue>(string key)
        {
            return await Task.Run(() =>
            {
                return GetValue<TValue>(key);
            });
        }
        #endregion

        #region public virtual TValue GetValue<TValue>(TAppSettings appSettings, string key)
        /// <summary>
        /// Pobierz wartość ustawienia z pliku konfiguracji .json na podstawie klucza
        /// Get the setting value from the .json configuration file based on the key
        /// </summary>
        /// <typeparam name="TValue">
        /// Typ zwracanej wartości
        /// The type of the return value
        /// </typeparam>
        /// <param name="appSettings">
        /// Referencja do instancji modelu klasy ustawień jako TAppSettings
        /// Reference to the settings class model instance as TAppSettings
        /// </param>
        /// <param name="key">
        /// Klucz
        /// Key
        /// </param>
        /// <returns>
        /// Wartość ustawienia na podstawie klucza jako typ określony w TValue
        /// The key-based setting value as the type specified in TValue
        /// </returns>
        public virtual TValue GetValue<TValue>(TAppSettings appSettings, string key)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(key) && null != appSettings.AppSettingsConfigurationRoot)
                {
                    return appSettings.AppSettingsConfigurationRoot.GetValue<TValue>(key);
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return GetDefaultValue<TValue>();
        }
        #endregion

        #region public virtual async Task<TValue> GetValueAsync<TValue>(TAppSettings appSettings, string key)
        /// <summary>
        /// Pobierz wartość ustawienia z pliku konfiguracji .json na podstawie klucza asynchronicznie
        /// Get the setting value from the .json configuration file based on the key asynchronously
        /// </summary>
        /// <typeparam name="TValue">
        /// Typ zwracanej wartości
        /// The type of the return value
        /// </typeparam>
        /// <param name="appSettings">
        /// Referencja do instancji modelu klasy ustawień jako TAppSettings
        /// Reference to the settings class model instance as TAppSettings
        /// </param>
        /// <param name="key">
        /// Klucz
        /// Key
        /// </param>
        /// <returns>
        /// Wartość ustawienia na podstawie klucza jako typ określony w TValue
        /// The key-based setting value as the type specified in TValue
        /// </returns>
        public virtual async Task<TValue> GetValueAsync<TValue>(TAppSettings appSettings, string key)
        {
            return await Task.Run(() =>
            {
                return GetValue<TValue>(appSettings, key);
            });
        }
        #endregion

        #region public virtual TValue GetValue<TValue>(string filePath, string key)
        /// <summary>
        /// Pobierz wartość ustawienia z pliku konfiguracji .json na podstawie klucza
        /// Get the setting value from the .json configuration file based on the key
        /// </summary>
        /// <typeparam name="TValue">
        /// Typ zwracanej wartości
        /// The type of the return value
        /// </typeparam>
        /// <param name="filePath">
        /// Absolutna ścieżka do pliku ustawień jako string
        /// Absolute path to the settings file as a string
        /// </param>
        /// <param name="key">
        /// Klucz
        /// Key
        /// </param>
        /// <returns>
        /// Wartość ustawienia na podstawie klucza jako typ określony w TValue
        /// The key-based setting value as the type specified in TValue
        /// </returns>
        public virtual TValue GetValue<TValue>(string filePath, string key)
        {
            try
            {
                if (null != filePath && !string.IsNullOrEmpty(filePath) && File.Exists(filePath) && null != key && !string.IsNullOrWhiteSpace(key))
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(filePath)).AddJsonFile(Path.GetFileName(filePath), optional: true, reloadOnChange: true);
                    IConfigurationRoot configurationRoot = configurationBuilder.Build();
                    if (null != configurationRoot)
                    {
                        return configurationRoot.GetValue<TValue>(key);
                    }
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
            return GetDefaultValue<TValue>();
        }
        #endregion

        #region public virtual async Task<TValue> GetValueAsync<TValue>(string filePath, string key)
        /// <summary>
        /// Pobierz wartość ustawienia z pliku konfiguracji .json na podstawie klucza asynchronicznie
        /// Get the setting value from the .json configuration file based on the key asynchronously
        /// </summary>
        /// <typeparam name="TValue">
        /// Typ zwracanej wartości
        /// The type of the return value
        /// </typeparam>
        /// <param name="filePath">
        /// Absolutna ścieżka do pliku ustawień jako string
        /// Absolute path to the settings file as a string
        /// </param>
        /// <param name="key">
        /// Klucz
        /// Key
        /// </param>
        /// <returns>
        /// Wartość ustawienia na podstawie klucza jako typ określony w TValue
        /// The key-based setting value as the type specified in TValue
        /// </returns>
        public virtual async Task<TValue> GetValueAsync<TValue>(string filePath, string key)
        {
            return await Task.Run(() =>
            {
                return GetValue<TValue>(filePath, key);
            });
        }
        #endregion
    }
    #endregion
}
