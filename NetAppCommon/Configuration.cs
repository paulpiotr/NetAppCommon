using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static System.Environment;

namespace NetAppCommon
{
    /// <summary>
    /// Klasa wspólna dla parametrów
    /// Class common to parameters
    /// </summary>
    public class Configuration
    {
        #region private readonly log4net.ILog log4net
        /// <summary>
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog Log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public static string SpecialFoldeGetFolderPath(string key)
        /// <summary>
        /// Pobierz ścieżkę do katalogu specjalnego
        /// Get the path to the special directory
        /// </summary>
        /// <param name="key">
        /// Wyszukiwany klucz jako string
        /// Search key as string
        /// </param>
        /// <returns>
        /// Ścieżka do katalogu specjalnego SpecialFolder jako string
        /// Path to the SpecialFolder special directory as a string
        /// </returns>
        public static string SpecialFoldeGetFolderPath(string key)
        {
            try
            {
                if (Enum.TryParse(key, true, out SpecialFolder specialFolder))
                {
                    try
                    {
                        return GetFolderPath(specialFolder);
                    }
                    catch (Exception e)
                    {
                        Log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return null;
        }
        #endregion

        #region public static string GetBaseDirectory()
        /// <summary>
        /// Pobierz podstawową (bazową) ścieżkę aplikacji
        /// Get the application's base path
        /// </summary>
        /// <returns>
        /// Podstawowa ścieżka aplikacji jako string lub null
        /// Base application path as string or null
        /// </returns>
        public static string GetBaseDirectory()
        {
            try
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return null;
        }
        #endregion

        #region public static string GetUserProfileDirectory()
        /// <summary>
        /// Pobierz podstawowy folder (katalog) użytkownika
        /// Get the application's base path
        /// </summary>
        /// <returns>
        /// Podstawowa ścieżka aplikacji jako string lub null
        /// Base application path as string or null
        /// </returns>
        public static string GetUserProfileDirectory()
        {
            try
            {
                var specialFolderUserProfile = Path.Combine(GetFolderPath(SpecialFolder.UserProfile), Assembly.GetCallingAssembly().GetName().Name);
                return specialFolderUserProfile ?? GetBaseDirectory();
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return null;
        }
        #endregion

        #region public static async Task<string> GetBaseDirectoryAsync()
        /// <summary>
        /// Pobierz podstawową (bazową) ścieżkę aplikacji asynchronicznie
        /// Get the application's base path asynchronously
        /// </summary>
        /// <returns>
        /// Podstawowa ścieżka aplikacji jako string lub null
        /// Base application path as string or null
        /// </returns>
        public static async Task<string> GetBaseDirectoryAsync()
        {
            try
            {
                await Task.Run(() => GetBaseDirectory());
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            }
            return null;
        }
        #endregion

        #region public static string GetAppSettingsPath(string settingsJsonFilePath = null)
        /// <summary>
        /// Pobierz ścieżkę do pliku konfiguracji.
        /// Get the path to the configuration file.
        /// </summary>
        /// <param name="settingsJsonFilePath">
        /// Nazwa pliku .json
        /// The name of the .json file
        /// </param>
        /// <returns>
        /// Ścieżka do pliku konfiguracji String lub null
        /// Path to the String configuration file or null
        /// </returns>
        public static string GetAppSettingsPath(string settingsJsonFilePath = null)
        {
            try
            {
                //log4net.Debug(string.Format("GetExecutingAssembly {0}, GetCallingAssembly {1} GetEntryAssembly {2}", Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetCallingAssembly().GetName().Name, Assembly.GetEntryAssembly().GetName().Name));
                if (File.Exists(settingsJsonFilePath) && Directory.Exists(Path.GetDirectoryName(settingsJsonFilePath)))
                {
                    return settingsJsonFilePath;
                }
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (Directory.Exists(baseDirectory))
                {
                    /// W pierwszej kolejności sprawdź czy istnieje plik appsettings.json w katalogu głównym projektu.
                    /// First, check if there is an settingsJsonFilePath file in the project's root directory
                    if (null != settingsJsonFilePath && File.Exists(Path.Combine(baseDirectory, settingsJsonFilePath)))
                    {
                        return Path.Combine(baseDirectory, settingsJsonFilePath);
                    }
                    /// W pierwszej kolejności sprawdź czy istnieje plik appsettings.json w katalogu głównym projektu.
                    /// Else, check if there is an appsettings.json file in the project's root directory
                    else if (File.Exists(Path.Combine(baseDirectory, "appsettings.json")))
                    {
                        return Path.Combine(baseDirectory, "appsettings.json");
                    }
                    /// W następnej kolejności sprawdz, czy istnieje plik {Nazwa Assembly}.json - przestrzeń nazw wykonywana
                    /// Next, check that the {Assembly Name} .json file being executed exists
                    else if (File.Exists(Path.Combine(baseDirectory, string.Format("{0}.json", Assembly.GetExecutingAssembly().GetName().Name))))
                    {
                        return Path.Combine(baseDirectory, string.Format("{0}.json", Assembly.GetExecutingAssembly().GetName().Name));
                    }
                    /// W następnej kolejności sprawdź, czy istnieje plik {Nazwa Assembly}.json - przestrzeń nazw wywołująca
                    /// In order, check that the {Assembly Name} .json file exists- calling namespace
                    else if (File.Exists(Path.Combine(baseDirectory, string.Format("{0}.json", Assembly.GetCallingAssembly().GetName().Name))))
                    {
                        return Path.Combine(baseDirectory, string.Format("{0}.json", Assembly.GetCallingAssembly().GetName().Name));
                    }
                    /// W następnej kolejności sprawdź, czy istnieje plik {Nazwa Assembly}.json - przestrzeń nazw wykonująca
                    /// In order, check that the {Assembly Name} .json file exists- calling entry
                    else if (File.Exists(Path.Combine(baseDirectory, string.Format("{0}.json", Assembly.GetEntryAssembly().GetName().Name))))
                    {
                        return Path.Combine(baseDirectory, string.Format("{0}.json", Assembly.GetEntryAssembly().GetName().Name));
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return null;
        }
        #endregion

        #region public static async Task<string> GetAppSettingsPathAsync(string settingsJsonFilePath = null)
        /// <summary>
        /// Pobierz ścieżkę do pliku konfiguracji asynchronicznie.
        /// Get the path to the configuration file asynchronously.
        /// </summary>
        /// <param name="settingsJsonFilePath">
        /// Nazwa pliku .json
        /// The name of the .json file
        /// </param>
        /// <returns>
        /// Ścieżka do pliku konfiguracji String lub null
        /// Path to the String configuration file or null
        /// </returns>
        public static async Task<string> GetAppSettingsPathAsync(string settingsJsonFilePath = null)
        {
            try
            {
                return await Task.Run(() => GetAppSettingsPath(settingsJsonFilePath));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }
        #endregion

        #region public static IConfigurationRoot GetConfigurationRoot()
        /// <summary>
        /// Pobierz Objekt Konfiguracji IConfigurationRoot ConfigurationRoot
        /// Get ConfigurationRoot Configuration Object
        /// </summary>
        /// <returns>
        /// Objekt Konfiguracji IConfigurationRoot ConfigurationRoot lub null
        /// IConfigurationRoot ConfigurationRoot configuration Object or null
        /// </returns>
        public static IConfigurationRoot GetConfigurationRoot()
        {
            try
            {
                var getAppSettingsPath = GetAppSettingsPath();
                if (null != getAppSettingsPath && !string.IsNullOrWhiteSpace(getAppSettingsPath))
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(getAppSettingsPath)).AddJsonFile(Path.GetFileName(getAppSettingsPath), optional: true, reloadOnChange: true);
                    IConfigurationRoot configurationRoot = configurationBuilder.Build();
                    return configurationRoot;
                }
                return null;
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
                return null;
            }
        }
        #endregion

        #region public static async Task<IConfigurationRoot> GetConfigurationRootAsync()
        /// <summary>
        /// Pobierz Objekt Konfiguracji IConfigurationRoot ConfigurationRoot asynchronicznie
        /// Get ConfigurationRoot Configuration Object asynchronously
        /// </summary>
        /// <returns>
        /// Objekt Konfiguracji IConfigurationRoot ConfigurationRoot lub null
        /// IConfigurationRoot ConfigurationRoot configuration Object or null
        /// </returns>
        public static async Task<IConfigurationRoot> GetConfigurationRootAsync()
        {
            try
            {
                return await Task.Run(() => GetConfigurationRoot());
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }
        #endregion

        #region public static IConfigurationRoot GetConfigurationRoot(string settingsJsonFilePath)
        /// <summary>
        /// Pobierz Objekt Konfiguracji IConfigurationRoot ConfigurationRoot asynchronicznie
        /// Get ConfigurationRoot Configuration Object asynchronously
        /// </summary>
        /// <param name="settingsJsonFilePath">
        /// Nazwa pliku .json
        /// The name of the .json file
        /// </param>
        /// <returns>
        /// Objekt Konfiguracji IConfigurationRoot ConfigurationRoot lub null
        /// IConfigurationRoot ConfigurationRoot configuration Object or null
        /// </returns>
        public static IConfigurationRoot GetConfigurationRoot(string settingsJsonFilePath)
        {
            try
            {
                var getAppSettingsPath = GetAppSettingsPath(settingsJsonFilePath);
                if (null != getAppSettingsPath && !string.IsNullOrWhiteSpace(getAppSettingsPath))
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(getAppSettingsPath)).AddJsonFile(Path.GetFileName(getAppSettingsPath), optional: true, reloadOnChange: true);
                    IConfigurationRoot configurationRoot = configurationBuilder.Build();
                    return configurationRoot;
                }
                return null;
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
                return null;
            }
        }
        #endregion

        #region public static async Task<IConfigurationRoot> GetConfigurationRootAsync(string settingsJsonFilePath)
        /// <summary>
        /// Pobierz Objekt Konfiguracji IConfigurationRoot ConfigurationRoot asynchronicznie
        /// Get ConfigurationRoot Configuration Object asynchronously
        /// </summary>
        /// <param name="settingsJsonFilePath">
        /// Nazwa pliku .json
        /// The name of the .json file
        /// </param>
        /// <returns>
        /// Objekt Konfiguracji IConfigurationRoot ConfigurationRoot lub null
        /// IConfigurationRoot ConfigurationRoot configuration Object or null
        /// </returns>
        public static async Task<IConfigurationRoot> GetConfigurationRootAsync(string settingsJsonFilePath)
        {
            try
            {
                return await Task.Run(() => GetConfigurationRoot(settingsJsonFilePath));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }
        #endregion

        #region public static T GetValue<T>(string key)
        /// <summary>
        /// Wyszukaj ustawienia aplikacji w pliku konfiguracji.
        /// Search for application settings in the configuration file.
        /// </summary>
        /// <typeparam name="T">
        /// Typ parametru.
        /// Parameter type.
        /// </typeparam>
        /// <param name="key">
        /// Szukany klucz.
        /// The key you are looking for.
        /// </param>
        /// <returns>
        /// Wartość ustawień jako typ T lub null jako typ T.
        /// Setting value as type T or null as type T.
        /// </returns>
        public static T GetValue<T>(string key)
        {
            try
            {
                if (null != key && !string.IsNullOrWhiteSpace(key))
                {
                    IConfigurationRoot configurationRoot = GetConfigurationRoot();
                    if (null != configurationRoot)
                    {
                        return (T)Convert.ChangeType(configurationRoot.GetValue<T>(key), typeof(T));
                    }
                }
                return (T)Convert.ChangeType(null, typeof(T));
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }
        #endregion

        #region public static async Task<T> GetValueAsync<T>(string key)
        /// <summary>
        /// Wyszukaj ustawienia aplikacji w pliku konfiguracji - asynchronicznie.
        /// Search for application settings in the configuration file - asynchronously.
        /// </summary>
        /// <typeparam name="T">
        /// Typ parametru.
        /// Parameter type.
        /// </typeparam>
        /// <param name="key">
        /// Szukany klucz.
        /// The key you are looking for.
        /// </param>
        /// <returns>
        /// Wartość ustawień jako typ T lub null jako typ T.
        /// Setting value as type T or null as type T.
        /// </returns>
        public static async Task<T> GetValueAsync<T>(string key)
        {
            try
            {
                return await Task.Run(() => GetValue<T>(key));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }
        #endregion

        #region public static T GetValue<T>(string settingsJsonFilePath, string key)
        /// <summary>
        /// Wyszukaj ustawienia aplikacji w pliku konfiguracji.
        /// Search for application settings in the configuration file.
        /// </summary>
        /// <typeparam name="T">
        /// Typ parametru.
        /// Parameter type.
        /// </typeparam>
        /// <param name="settingsJsonFilePath">
        /// Nazwa pliku .json
        /// The name of the .json file
        /// </param>
        /// <param name="key">
        /// Szukany klucz.
        /// The key you are looking for.
        /// </param>
        /// <returns>
        /// Wartość ustawień jako typ T lub null jako typ T.
        /// Setting value as type T or null as type T.
        /// </returns>
        public static T GetValue<T>(string settingsJsonFilePath, string key)
        {
            try
            {
                if (null != settingsJsonFilePath && !string.IsNullOrEmpty(settingsJsonFilePath) && null != key && !string.IsNullOrWhiteSpace(key))
                {
                    IConfigurationRoot configurationRoot = GetConfigurationRoot(settingsJsonFilePath);
                    if (null != configurationRoot)
                    {
                        return (T)Convert.ChangeType(configurationRoot.GetValue<T>(key), typeof(T));
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }
        #endregion

        #region public static async Task<T> GetValueAsync<T>(string settingsJsonFilePath, string key)
        /// <summary>
        /// Wyszukaj ustawienia aplikacji w pliku konfiguracji - asynchronicznie.
        /// Search for application settings in the configuration file - asynchronously.
        /// </summary>
        /// <typeparam name="T">
        /// Typ parametru.
        /// Parameter type.
        /// </typeparam>
        /// <param name="settingsJsonFilePath">
        /// Nazwa pliku .json
        /// The name of the .json file
        /// </param>
        /// <param name="key">
        /// Szukany klucz.
        /// The key you are looking for.
        /// </param>
        /// <returns>
        /// Wartość ustawień jako typ T lub null jako typ T.
        /// Setting value as type T or null as type T.
        /// </returns>
        public static async Task<T> GetValueAsync<T>(string settingsJsonFilePath, string key)
        {
            try
            {
                return await Task.Run(() => GetValue<T>(settingsJsonFilePath, key));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }
        #endregion

        #region public static void SaveConfigurationToFile<T>(T _object, string appSettingsPath = null)
        /// <summary>
        /// Zapisz kofigurację do pliku
        /// Save configuration to file
        /// </summary>
        /// <typeparam name="T">
        /// Typ obiektu jako parametr typu T
        /// Object type as a parameter of type T
        /// </typeparam>
        /// <param name="_object">
        /// Obiekt typu parametru T
        /// Object of the T parameter type
        /// </param>
        /// <param name="appSettingsPath">
        /// Opcjonalnie ścieżka do pliku jako string
        /// Optional file path as string
        /// </param>
        public static void SaveConfigurationToFile<T>(T _object, string appSettingsPath = null)
        {
            try
            {
                if (null == appSettingsPath)
                {
                    appSettingsPath = GetAppSettingsPath();
                }
                if (null != appSettingsPath && !string.IsNullOrWhiteSpace(appSettingsPath) && File.Exists(appSettingsPath))
                {
                    var json = JsonConvert.SerializeObject(_object, Formatting.Indented);
                    if (null != json)
                    {
                        File.WriteAllText(appSettingsPath, json);
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
        }
        #endregion

        #region public static async Task SaveConfigurationToFileAsync<T>(T _object, string appSettingsPath = null)
        /// <summary>
        /// Zapisz kofigurację do pliku asynchronicznie
        /// Save configuration to file asynchronously
        /// </summary>
        /// <typeparam name="T">
        /// Typ obiektu jako parametr typu T
        /// Object type as a parameter of type T
        /// </typeparam>
        /// <param name="_object">
        /// Obiekt typu parametru T
        /// Object of the T parameter type
        /// </param>
        /// <param name="appSettingsPath">
        /// Opcjonalnie ścieżka do pliku jako string
        /// Optional file path as string
        /// </param>
        public static async Task SaveConfigurationToFileAsync<T>(T _object, string appSettingsPath = null)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (null == appSettingsPath)
                    {
                        appSettingsPath = GetAppSettingsPath();
                    }
                    if (null != appSettingsPath && !string.IsNullOrWhiteSpace(appSettingsPath) && File.Exists(appSettingsPath))
                    {
                        var json = JsonConvert.SerializeObject(_object, Formatting.Indented);
                        if (null != json)
                        {
                            File.WriteAllText(appSettingsPath, json);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            }
        }
        #endregion

        #region public static void SetAppSettingValue<T>(string key, T value, string appSettingsPath = null)
        /// <summary>
        /// Zapisz ustawienia wegług klucza key jako typ T do pliku ustawień.
        /// Save the settings by key as type T to the settings file.
        /// </summary>
        /// <param name="key">
        /// Klucz String
        /// The String key
        /// </param>
        /// <param name="value">
        /// Nowa wartość jako T.
        /// New value as T.
        /// </param>
        /// <param name="appSettingsPath">
        /// Ścieżka do pliku ustawień String
        /// Path to the settings file String
        /// </param>
        public static void SetAppSettingValue<T>(string key, T value, string appSettingsPath = null)
        {
            try
            {
                if (null == appSettingsPath)
                {
                    appSettingsPath = GetAppSettingsPath();
                }
                if (null != appSettingsPath && !string.IsNullOrWhiteSpace(appSettingsPath) && File.Exists(appSettingsPath))
                {
                    var json = File.ReadAllText(appSettingsPath);
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
                    if (null != jsonObj)
                    {
                        jsonObj[key] = value;
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        if (null != output)
                        {
                            File.WriteAllText(appSettingsPath, output);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
        }
        #endregion

        #region public static async Task SetAppSettingValueAsync<T>(string key, T value, string appSettingsPath = null)
        /// <summary>
        /// Zapisz ustawienia wegług klucza key jako typ T do pliku ustawień - asynchronicznie.
        /// Save the settings by key as type T to the settings file - asynchronously.
        /// </summary>
        /// <param name="key">
        /// Klucz String
        /// The String key
        /// </param>
        /// <param name="value">
        /// Nowa wartość jako T.
        /// New value as T.
        /// </param>
        /// <param name="appSettingsPath">
        /// Ścieżka do pliku ustawień String
        /// Path to the settings file String
        /// </param>
        public static async Task SetAppSettingValueAsync<T>(string key, T value, string appSettingsPath = null)
        {
            try
            {
                await Task.Run(() => SetAppSettingValue<T>(key: key, value: value, appSettingsPath));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            }
        }
        #endregion
    }
}
