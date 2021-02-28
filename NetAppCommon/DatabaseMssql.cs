#region using

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

#endregion

namespace NetAppCommon
{
    /// <summary>
    ///     Klasa wspólna do połączeń z bazą danych
    ///     Common class for connections to data
    /// </summary>
    public class DatabaseMssql
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     private readonly ILog _log4Net
        /// </summary>
        private static readonly ILog Log4net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public static string ParseConnectionString(string connectionString)

        /// <summary>
        ///     Parsuj połączenie do bazy danych i zastąp zmiennymi systemowymi
        /// </summary>
        /// <param name="connectionString">
        ///     Ciąg połączenia do bazy danych As String
        ///     Database connection string As String
        /// </param>
        /// <returns>
        ///     Ciąg połączenia do bazy danych As String
        ///     Database connection string As String
        /// </returns>
        public static string ParseConnectionString(string connectionString)
        {
            try
            {
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    //log4net.Debug(connectionString);
                    //Regex regex = new Regex(@"%.*?%");
                    MatchCollection matchCollection = new Regex(@"%.*?%").Matches(connectionString);
                    foreach (Match match in matchCollection)
                    {
                        if (!string.IsNullOrWhiteSpace(match.Value))
                        {
                            var stringType = match.Value.Replace("%", string.Empty);
                            //log4net.Debug(match.Value);
                            //log4net.Debug(stringType);
                            if (
                                (stringType.Contains("System.Environment.GetFolderPath") ||
                                 stringType.Contains("Environment.GetFolderPath")) &&
                                (stringType.Contains("System.Environment.SpecialFolder") ||
                                 stringType.Contains("Environment.SpecialFolder"))
                            )
                            {
                                var key = stringType.Replace(".", string.Empty).Replace("(", string.Empty)
                                    .Replace(")", string.Empty).Replace("System", string.Empty)
                                    .Replace("Environment", string.Empty).Replace("GetFolderPath", string.Empty)
                                    .Replace("SpecialFolder", string.Empty);
                                try
                                {
                                    connectionString = connectionString.Replace(match.Value,
                                        Configuration.SpecialFoldeGetFolderPath(key));
                                }
                                catch (Exception e)
                                {
                                    Log4net.Error(
                                        $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n",
                                        e);
                                }
                            }
                        }
                    }

                    if (connectionString.Contains("%AppDomain.CurrentDomain.BaseDirectory%"))
                    {
                        connectionString = connectionString.Replace("%AppDomain.CurrentDomain.BaseDirectory%",
                            AppDomain.CurrentDomain.BaseDirectory);
                    }

                    ///GetExecutingAssembly
                    if (connectionString.Contains("%Assembly.GetExecutingAssembly().GetName().Name%"))
                    {
                        connectionString = connectionString.Replace("%Assembly.GetExecutingAssembly().GetName().Name%",
                            Assembly.GetExecutingAssembly().GetName().Name);
                    }

                    if (connectionString.Contains("%System.Reflection.Assembly.GetExecutingAssembly().GetName().Name%"))
                    {
                        connectionString = connectionString.Replace(
                            "%System.Reflection.Assembly.GetExecutingAssembly().GetName().Name%",
                            Assembly.GetExecutingAssembly().GetName().Name);
                    }

                    ///GetCallingAssembly
                    if (connectionString.Contains("%Assembly.GetCallingAssembly().GetName().Name%"))
                    {
                        connectionString = connectionString.Replace("%Assembly.GetCallingAssembly().GetName().Name%",
                            Assembly.GetCallingAssembly().GetName().Name);
                    }

                    if (connectionString.Contains("%System.Reflection.Assembly.GetCallingAssembly().GetName().Name%"))
                    {
                        connectionString = connectionString.Replace(
                            "%System.Reflection.Assembly.GetCallingAssembly().GetName().Name%",
                            Assembly.GetCallingAssembly().GetName().Name);
                    }

                    ///GetEntryAssembly
                    if (connectionString.Contains("%Assembly.GetEntryAssembly().GetName().Name%"))
                    {
                        connectionString = connectionString.Replace("%Assembly.GetEntryAssembly().GetName().Name%",
                            Assembly.GetEntryAssembly()?.GetName().Name);
                    }

                    if (connectionString.Contains("%System.Reflection.Assembly.GetEntryAssembly().GetName().Name%"))
                    {
                        connectionString = connectionString.Replace(
                            "%System.Reflection.Assembly.GetEntryAssembly().GetName().Name%",
                            Assembly.GetEntryAssembly()?.GetName().Name);
                    }

                    if (connectionString.Contains("%AttachDbFilename%"))
                    {
                        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
                        if (null != sqlConnectionStringBuilder.AttachDBFilename &&
                            !string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.AttachDBFilename))
                        {
                            var stringBuilder = new StringBuilder();
                            var prefix = Path.GetFileName(sqlConnectionStringBuilder.AttachDBFilename)
                                .Replace(Path.GetExtension(sqlConnectionStringBuilder.AttachDBFilename), string.Empty);
                            stringBuilder.Append(!string.IsNullOrWhiteSpace(prefix) ? prefix : "MSSQLLocalDB");
                            stringBuilder.Append("-");
                            stringBuilder.Append(new Guid(MD5.Create()
                                .ComputeHash(Encoding.ASCII.GetBytes(sqlConnectionStringBuilder.AttachDBFilename))));
                            var attachDbFilename = Regex.Replace(stringBuilder.ToString().ToLower(), "[^A-Za-z0-9]", string.Empty);
                            connectionString = connectionString.Replace("%AttachDbFilename%",
                                attachDbFilename);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return connectionString;
        }

        #endregion

        #region public static string GetConnectionString(string connectionStringName, string settingsJsonFileName = null)

        /// <summary>
        ///     Pobierz parametry połączenia
        ///     Get the connection string
        /// </summary>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <param name="settingsJsonFileName">
        ///     Nazwa pliku .json
        ///     The name of the .json file
        /// </param>
        /// <returns>
        ///     Parametry połączenia jako string lub null
        ///     Connection string as string or null
        /// </returns>
        public static string GetConnectionString(string connectionStringName, string settingsJsonFileName = null)
        {
            try
            {
                return ParseConnectionString(Configuration.GetConfigurationRoot(settingsJsonFileName)
                    .GetConnectionString(connectionStringName));
            }
            catch (Exception e)
            {
                Log4net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                return null;
            }
        }

        #endregion

        #region public static async Task<string> GetConnectionStringAsync(string connectionStringName, string settingsJsonFileName = null)

        /// <summary>
        ///     Pobierz parametry połączenia asynchronicznie
        ///     Get the connection string asynchronously
        /// </summary>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <param name="settingsJsonFileName">
        ///     Nazwa pliku .json
        ///     The name of the .json file
        /// </param>
        /// <returns>
        ///     Parametry połączenia String lub null
        ///     A String or null connection string
        /// </returns>
        public static async Task<string> GetConnectionStringAsync(string connectionStringName,
            string settingsJsonFileName = null)
        {
            try
            {
                return await Task.Run(() => GetConnectionString(connectionStringName, settingsJsonFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        #endregion

        #region public static string GetDecryptConnectionString(string connectionStringName, string settingsJsonFileName = null)

        /// <summary>
        ///     Pobierz i odszyfruj połączenie do bazy danych
        ///     Get and decrypt the connection to the database
        /// </summary>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Parametry połączenia String lub null
        ///     A String or null connection string
        /// </returns>
        public static string GetDecryptConnectionString(string connectionStringName, string settingsJsonFileName = null)
        {
            try
            {
                var connectionString = GetConnectionString(connectionStringName, settingsJsonFileName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString =
                        ParseConnectionString(
                            string.Empty /*EncryptDecrypt.EncryptDecrypt.DecryptString(connectionString, EncryptDecrypt.EncryptDecrypt.GetRsaFileContent())*/);
                }

                return connectionString;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
                return null;
            }
        }

        #endregion

        #region public static async Task<string> GetDecryptConnectionStringAsync(string connectionStringName, string settingsJsonFileName = null)

        /// <summary>
        ///     Pobierz i odszyfruj połączenie do bazy danych asynchronicznie
        ///     Get and decrypt the connection to the database asynchronously
        /// </summary>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Parametry połączenia String lub null
        ///     A String or null connection string
        /// </returns>
        public static async Task<string> GetDecryptConnectionStringAsync(string connectionStringName,
            string settingsJsonFileName = null)
        {
            try
            {
                return await Task.Run(() => GetDecryptConnectionString(connectionStringName, settingsJsonFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        #endregion

        #region public static string GetDecryptConnectionString(string connectionStringName, string rsaFileName, string settingsJsonFileName = null)

        /// <summary>
        ///     Pobierz i odszyfruj połączenie do bazy danych
        ///     Get and decrypt the connection to the database
        /// </summary>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <param name="rsaFileName">
        ///     Nazwa pliku szyfrującego
        ///     The name of the encryption file
        /// </param>
        /// <returns>
        ///     Parametry połączenia String lub null
        ///     A String or null connection string
        /// </returns>
        public static string GetDecryptConnectionString(string connectionStringName, string rsaFileName,
            string settingsJsonFileName = null)
        {
            try
            {
                var connectionString = GetConnectionString(connectionStringName, settingsJsonFileName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString =
                        ParseConnectionString(
                            string.Empty /*EncryptDecrypt.EncryptDecrypt.DecryptString(connectionString, EncryptDecrypt.EncryptDecrypt.GetRsaFileContent(rsaFileName))*/);
                }

                return connectionString;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
                return null;
            }
        }

        #endregion

        #region public static async Task<string> GetDecryptConnectionStringAsync...

        /// <summary>
        ///     Pobierz i odszyfruj połączenie do bazy danych asynchronicznie
        ///     Get and decrypt the connection to the database asynchronously
        /// </summary>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <param name="rsaFileName">
        ///     Nazwa pliku szyfrującego
        ///     The name of the encryption file
        /// </param>
        /// <returns>
        ///     Parametry połączenia String lub null
        ///     A String or null connection string
        /// </returns>
        public static async Task<string> GetDecryptConnectionStringAsync(string connectionStringName,
            string rsaFileName, string settingsJsonFileName = null)
        {
            try
            {
                return await Task.Run(() =>
                    GetDecryptConnectionString(connectionStringName, rsaFileName, settingsJsonFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        #endregion

        /// <summary>
        ///     Odszyfruj i pobierz opcje budowania kontekstu serwera Sql
        ///     Decrypt and retrieve Sql Server context build options
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Opcje budowania kontekstu serwera Sql
        ///     Sql Server Context Build Options
        /// </returns>
        public static DbContextOptions GetSqlServerDbContextOptions<T>(string connectionStringName,
            string settingsJsonFileName = null) where T : DbContext
        {
            try
            {
                DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder<T>();
                var connectionString = GetConnectionString(connectionStringName, settingsJsonFileName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    dbContextOptionsBuilder.UseSqlServer(connectionString);
                    return (DbContextOptions<T>)dbContextOptionsBuilder.Options;
                }

                return null;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                return null;
            }
        }

        /// <summary>
        ///     Pobierz opcje budowania kontekstu serwera Sql asynchronicznie
        ///     Get Sql Server context build options asynchronously
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Opcje budowania kontekstu serwera Sql
        ///     Sql Server Context Build Options
        /// </returns>
        public static async Task<DbContextOptions> GetSqlServerDbContextOptionsAsync<T>(string connectionStringName,
            string settingsJsonFileName = null) where T : DbContext
        {
            try
            {
                return await Task.Run(() =>
                    GetSqlServerDbContextOptions<T>(connectionStringName, settingsJsonFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        ///     Pobierz opcje budowania kontekstu serwera Sql
        ///     Get the Sql Server context build options
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Opcje budowania kontekstu serwera Sql
        ///     Sql Server Context Build Options
        /// </returns>
        public static DbContextOptions GetDecryptSqlServerDbContextOptions<T>(string connectionStringName)
            where T : DbContext
        {
            try
            {
                DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder<T>();
                var connectionString = GetDecryptConnectionString(connectionStringName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    dbContextOptionsBuilder.UseSqlServer(connectionString);
                    return (DbContextOptions<T>)dbContextOptionsBuilder.Options;
                }

                return null;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        ///     Pobierz opcje budowania kontekstu serwera Sql asynchronicznie
        ///     Get the Sql Server context build options asynchronously
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Opcje budowania kontekstu serwera Sql
        ///     Sql Server Context Build Options
        /// </returns>
        public static async Task<DbContextOptions> GetDecryptSqlServerDbContextOptionsAsync<T>(
            string connectionStringName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => GetDecryptSqlServerDbContextOptions<T>(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        ///     Pobierz opcje budowania kontekstu serwera Sql
        ///     Get the Sql Server context build options
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <param name="rsaFileName">
        ///     Nazwa pliku szyfrującego
        ///     The name of the encryption file
        /// </param>
        /// <returns>
        ///     Opcje budowania kontekstu serwera Sql
        ///     Sql Server Context Build Options
        /// </returns>
        public static DbContextOptions GetDecryptSqlServerDbContextOptions<T>(string connectionStringName,
            string rsaFileName) where T : DbContext
        {
            try
            {
                DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder<T>();
                var connectionString = GetDecryptConnectionString(connectionStringName, rsaFileName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    dbContextOptionsBuilder.UseSqlServer(connectionString);
                    return (DbContextOptions<T>)dbContextOptionsBuilder.Options;
                }

                return null;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        ///     Pobierz opcje budowania kontekstu serwera Sql asynchronicznie
        ///     Get the Sql Server context build options asynchronously
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <param name="rsaFileName">
        ///     Nazwa pliku szyfrującego
        ///     The name of the encryption file
        /// </param>
        /// <returns>
        ///     Opcje budowania kontekstu serwera Sql
        ///     Sql Server Context Build Options
        /// </returns>
        public static async Task<DbContextOptions> GetDecryptSqlServerDbContextOptionsAsync<T>(
            string connectionStringName, string rsaFileName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => GetDecryptSqlServerDbContextOptions<T>(connectionStringName, rsaFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        ///     Utwórz instancje do klasy kontekstowej bazy danych
        ///     Create instances for the database context class
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <returns>
        ///     Instancja do klasy kontekstowej bazy danych lub null
        ///     An instance to the database context class or null
        /// </returns>
        public static T CreateInstancesForDatabaseContextClass<T>() where T : DbContext
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        ///     Utwórz instancje do klasy kontekstowej bazy danych asynchronicznie
        ///     Create instances for the database context class asynchronously
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <returns>
        ///     Instancja do klasy kontekstowej bazy danych lub null
        ///     An instance to the database context class or null
        /// </returns>
        public static async Task<T> CreateInstancesForDatabaseContextClassAsync<T>() where T : DbContext
        {
            try
            {
                return await Task.Run(() => CreateInstancesForDatabaseContextClass<T>());
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        ///     Utwórz instancje do klasy kontekstowej bazy danych
        ///     Create instances for the database context class
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Instancja do klasy kontekstowej bazy danych lub null
        ///     An instance to the database context class or null
        /// </returns>
        public static T CreateInstancesForDatabaseContextClass<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                DbContextOptions dbContextOptions = GetSqlServerDbContextOptions<T>(connectionStringName);
                if (null != dbContextOptions)
                {
                    object[] vs = {GetSqlServerDbContextOptions<T>(connectionStringName)};
                    return (T)Activator.CreateInstance(typeof(T), vs);
                }

                return null;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        ///     Utwórz instancje do klasy kontekstowej bazy danych asynchronicznie
        ///     Create instances for the database context class asynchronously
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Instancja do klasy kontekstowej bazy danych lub null
        ///     An instance to the database context class or null
        /// </returns>
        public static async Task<T> CreateInstancesForDatabaseContextClassAsync<T>(string connectionStringName)
            where T : DbContext
        {
            try
            {
                return await Task.Run(() => CreateInstancesForDatabaseContextClass<T>(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        ///     Odszyfruj i utwórz instancje do klasy kontekstowej bazy danych
        ///     Decrypt and create instances into the database context class
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Instancja do klasy kontekstowej bazy danych lub null
        ///     An instance to the database context class or null
        /// </returns>
        public static T DecryptAndCreateInstancesForDatabaseContextClass<T>(string connectionStringName)
            where T : DbContext
        {
            try
            {
                DbContextOptions dbContextOptions = GetDecryptSqlServerDbContextOptions<T>(connectionStringName);
                if (null != dbContextOptions)
                {
                    object[] vs = {GetDecryptSqlServerDbContextOptions<T>(connectionStringName)};
                    return (T)Activator.CreateInstance(typeof(T), vs);
                }

                return null;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        ///     Odszyfruj i utwórz instancje do klasy kontekstowej bazy danych asynchronicznie
        ///     Decrypt and create instances into the database context class asynchronously
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Instancja do klasy kontekstowej bazy danych lub null
        ///     An instance to the database context class or null
        /// </returns>
        public static async Task<T> DecryptAndCreateInstancesForDatabaseContextClassAsync<T>(
            string connectionStringName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => DecryptAndCreateInstancesForDatabaseContextClass<T>(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        ///     Odszyfruj i utwórz instancje do klasy kontekstowej bazy danych
        ///     Decrypt and create instances into the database context class
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Instancja do klasy kontekstowej bazy danych lub null
        ///     An instance to the database context class or null
        /// </returns>
        public static T DecryptAndCreateInstancesForDatabaseContextClass<T>(string connectionStringName,
            string rsaFileName) where T : DbContext
        {
            try
            {
                DbContextOptions dbContextOptions =
                    GetDecryptSqlServerDbContextOptions<T>(connectionStringName, rsaFileName);
                if (null != dbContextOptions)
                {
                    object[] vs = {dbContextOptions};
                    return (T)Activator.CreateInstance(typeof(T), vs);
                }

                return null;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        ///     Odszyfruj i utwórz instancje do klasy kontekstowej bazy danych asynchronicznie
        ///     Decrypt and create instances into the database context class asynchronously
        /// </summary>
        /// <typeparam name="T">
        ///     Typ kontekstu bazy danych
        ///     Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        ///     Nazwa ciągu połączenia String
        ///     The name of the connection String
        /// </param>
        /// <returns>
        ///     Instancja do klasy kontekstowej bazy danych lub null
        ///     An instance to the database context class or null
        /// </returns>
        public static async Task<T> DecryptAndCreateInstancesForDatabaseContextClassAsync<T>(
            string connectionStringName, string rsaFileName) where T : DbContext
        {
            try
            {
                return await Task.Run(() =>
                    DecryptAndCreateInstancesForDatabaseContextClass<T>(connectionStringName, rsaFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        #region public static bool CanConnect(string connectionString)

        /// <summary>
        ///     Sprawdź, czy można połączyćsięz bazą danych Mssql
        ///     Verify that you can connect to the Mssql database
        /// </summary>
        /// <param name="connectionString">
        ///     Ciąg połączenia do bazy danych Mssql jako string
        ///     The connection string to the Mssql database as a string
        /// </param>
        /// <returns>
        ///     Prawda jeśli udało się połączyć z bazą danych Mssql, przeciwnie fałsz
        ///     True if successful when connecting to the Mssql database, otherwise false
        /// </returns>
        public static bool CanConnect(string connectionString)
        {
            SqlConnection sqlConnection = null;
            try
            {
                using (sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    return true;
                }
            }
            catch (SqlException e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }
            finally
            {
                try
                {
                    if (null != sqlConnection)
                    {
                        sqlConnection.Close();
                    }
                }
                catch (Exception e)
                {
                    Log4net.Error(
                        string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                            e.StackTrace), e);
                }
            }

            return false;
        }

        #endregion

        #region public static async Task<bool> CanConnectAsync(string connectionString)

        /// <summary>
        ///     Sprawdź, czy można połączyćsięz bazą danych Mssql asynchronicznie
        ///     Verify that you can connect to the Mssql database asynchronously
        /// </summary>
        /// <param name="connectionString">
        ///     Ciąg połączenia do bazy danych Mssql jako string
        ///     The connection string to the Mssql database as a string
        /// </param>
        /// <returns>
        ///     Prawda jeśli udało się połączyć z bazą danych Mssql, przeciwnie fałsz
        ///     True if successful when connecting to the Mssql database, otherwise false
        /// </returns>
        public static async Task<bool> CanConnectAsync(string connectionString)
        {
            try
            {
                return await Task.Run(() => CanConnect(connectionString));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            }

            return false;
        }

        #endregion
    }
}
