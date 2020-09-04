using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace NetAppCommon
{
    /// <summary>
    /// Klasa wspólna dla parametrów
    /// Class common to parameters
    /// </summary>
    public class DataConfiguration
    {
        /// <summary>
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Pobierz ścieżkę do pliku konfiguracji.
        /// Get the path to the configuration file.
        /// </summary>
        /// <returns>
        /// Ścieżka do pliku konfiguracji String lub null
        /// Path to the String configuration file or null
        /// </returns>
        public static string GetAppSettingsPath()
        {
            try
            {
                //_log4net.Info(string.Format("GetExecutingAssembly {0}, GetCallingAssembly {1} GetEntryAssembly {2}", Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetCallingAssembly().GetName().Name, Assembly.GetEntryAssembly().GetName().Name));
                /// W pierwszej kolejności sprawdź czy istnieje plik appsettings.json w katalogu głównym projektu.
                /// First, check if there is an appsettings.json file in the project's root directory
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json")))
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                }
                /// W następnej kolejności sprawdz, czy istnieje plik {Nazwa Assembly}.json - przestrzeń nazw wykonywana
                /// Next, check that the {Assembly Name} .json file being executed exists
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.json", Assembly.GetExecutingAssembly().GetName().Name))))
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.json", Assembly.GetExecutingAssembly().GetName().Name));
                }
                /// W następnej kolejności sprawdź, czy istnieje plik {Nazwa Assembly}.json - przestrzeń nazw wywołująca
                /// In order, check that the {Assembly Name} .json file exists- calling namespace
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.json", Assembly.GetCallingAssembly().GetName().Name))))
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.json", Assembly.GetCallingAssembly().GetName().Name));
                }
                /// W następnej kolejności sprawdź, czy istnieje plik {Nazwa Assembly}.json - przestrzeń nazw wykonująca
                /// In order, check that the {Assembly Name} .json file exists- calling entry
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.json", Assembly.GetEntryAssembly().GetName().Name))))
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.json", Assembly.GetEntryAssembly().GetName().Name));
                }
                return null;
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        /// Pobierz ścieżkę do pliku konfiguracji asynchronicznie.
        /// Get the path to the configuration file asynchronously.
        /// </summary>
        /// <returns>
        /// Ścieżka do pliku konfiguracji String lub null
        /// Path to the String configuration file or null
        /// </returns>
        public static async Task<string> GetAppSettingsPathAsync()
        {
            try
            {
                return await Task.Run(() => GetAppSettingsPath());
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

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
                string getAppSettingsPath = GetAppSettingsPath();
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
                _log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
                return null;
            }
        }

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
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

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
                _log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }

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
            catch(Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }

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
                    string json = File.ReadAllText(appSettingsPath);
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
                _log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
        }

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
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            }
        }

    }
}
