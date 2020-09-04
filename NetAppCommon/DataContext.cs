using EncryptDecrypt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetAppCommon
{
    /// <summary>
    /// Klasa wspólna do połączeń z bazą danych
    /// Common class for connections to data
    /// </summary>
    public class DataContext
    {
        /// <summary>
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Parsuj połączenie do bazy danych i zastąp zmiennymi systemowymi
        /// </summary>
        /// <param name="connectionString">
        /// Ciąg połączenia do bazy danych As String
        /// Database connection string As String
        /// </param>
        /// <returns>
        /// Ciąg połączenia do bazy danych As String
        /// Database connection string As String
        /// </returns>
        public static string ParseConnectionString(string connectionString)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("%AppDomain.CurrentDomain.BaseDirectory%"))
                {
                    connectionString = connectionString.Replace("%AppDomain.CurrentDomain.BaseDirectory%", AppDomain.CurrentDomain.BaseDirectory);
                }
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("%Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)%"))
                {
                    connectionString = connectionString.Replace("%Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                }
                ///GetExecutingAssembly
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("%Assembly.GetExecutingAssembly().GetName().Name%"))
                {
                    connectionString = connectionString.Replace("%Assembly.GetExecutingAssembly().GetName().Name%", Assembly.GetExecutingAssembly().GetName().Name);
                }
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("%System.Reflection.Assembly.GetExecutingAssembly().GetName().Name%"))
                {
                    connectionString = connectionString.Replace("%System.Reflection.Assembly.GetExecutingAssembly().GetName().Name%", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                }
                ///GetCallingAssembly
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("%Assembly.GetCallingAssembly().GetName().Name%"))
                {
                    connectionString = connectionString.Replace("%Assembly.GetCallingAssembly().GetName().Name%", Assembly.GetCallingAssembly().GetName().Name);
                }
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("%System.Reflection.Assembly.GetCallingAssembly().GetName().Name%"))
                {
                    connectionString = connectionString.Replace("%System.Reflection.Assembly.GetCallingAssembly().GetName().Name%", System.Reflection.Assembly.GetCallingAssembly().GetName().Name);
                }
                ///GetEntryAssembly
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("%Assembly.GetEntryAssembly().GetName().Name%"))
                {
                    connectionString = connectionString.Replace("%Assembly.GetEntryAssembly().GetName().Name%", Assembly.GetEntryAssembly().GetName().Name);
                }
                if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("%System.Reflection.Assembly.GetEntryAssembly().GetName().Name%"))
                {
                    connectionString = connectionString.Replace("%System.Reflection.Assembly.GetEntryAssembly().GetName().Name%", System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
                }
                return connectionString;
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        /// Pobierz parametry połączenia
        /// Get the connection string
        /// </summary>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Parametry połączenia String lub null
        /// A String or null connection string
        /// </returns>
        public static string GetConnectionString(string connectionStringName)
        {
            try
            {
                return ParseConnectionString(DataConfiguration.GetConfigurationRoot().GetConnectionString(connectionStringName));
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        /// Pobierz parametry połączenia asynchronicznie
        /// Get the connection string asynchronously
        /// </summary>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Parametry połączenia String lub null
        /// A String or null connection string
        /// </returns>
        public static async Task<string> GetConnectionStringAsync(string connectionStringName)
        {
            try
            {
                return await Task.Run(() => GetConnectionString(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Pobierz i odszyfruj połączenie do bazy danych
        /// Get and decrypt the connection to the database
        /// </summary>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Parametry połączenia String lub null
        /// A String or null connection string
        /// </returns>
        public static string GetDecryptConnectionString(string connectionStringName)
        {
            try
            {
                string connectionString = GetConnectionString(connectionStringName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString = ParseConnectionString(EncryptDecrypt.EncryptDecrypt.DecryptString(connectionString, EncryptDecrypt.EncryptDecrypt.GetRsaFileContent()));
                }
                return connectionString;
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        /// Pobierz i odszyfruj połączenie do bazy danych asynchronicznie
        /// Get and decrypt the connection to the database asynchronously
        /// </summary>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Parametry połączenia String lub null
        /// A String or null connection string
        /// </returns>
        public static async Task<string> GetDecryptConnectionStringAsync(string connectionStringName)
        {
            try
            {
                return await Task.Run(() => GetDecryptConnectionString(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Pobierz i odszyfruj połączenie do bazy danych
        /// Get and decrypt the connection to the database
        /// </summary>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <param name="rsaFileName">
        /// Nazwa pliku szyfrującego
        /// The name of the encryption file
        /// </param>
        /// <returns>
        /// Parametry połączenia String lub null
        /// A String or null connection string
        /// </returns>
        public static string GetDecryptConnectionString(string connectionStringName, string rsaFileName)
        {
            try
            {
                string connectionString = GetConnectionString(connectionStringName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString = ParseConnectionString(EncryptDecrypt.EncryptDecrypt.DecryptString(connectionString, EncryptDecrypt.EncryptDecrypt.GetRsaFileContent(rsaFileName)));
                }
                return connectionString;
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        /// Pobierz i odszyfruj połączenie do bazy danych asynchronicznie
        /// Get and decrypt the connection to the database asynchronously
        /// </summary>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <param name="rsaFileName">
        /// Nazwa pliku szyfrującego
        /// The name of the encryption file
        /// </param>
        /// <returns>
        /// Parametry połączenia String lub null
        /// A String or null connection string
        /// </returns>
        public static async Task<string> GetDecryptConnectionStringAsync(string connectionStringName, string rsaFileName)
        {
            try
            {
                return await Task.Run(() => GetDecryptConnectionString(connectionStringName, rsaFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Odszyfruj i pobierz opcje budowania kontekstu serwera Sql
        /// Decrypt and retrieve Sql Server context build options
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Opcje budowania kontekstu serwera Sql 
        /// Sql Server Context Build Options
        /// </returns>
        public static DbContextOptions GetSqlServerDbContextOptions<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder<T>();
                string connectionString = GetConnectionString(connectionStringName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    dbContextOptionsBuilder.UseSqlServer(connectionString);
                    return (DbContextOptions<T>)dbContextOptionsBuilder.Options;
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
        /// Pobierz opcje budowania kontekstu serwera Sql asynchronicznie
        /// Get Sql Server context build options asynchronously
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Opcje budowania kontekstu serwera Sql 
        /// Sql Server Context Build Options
        /// </returns>
        public static async Task<DbContextOptions> GetSqlServerDbContextOptionsAsync<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => GetSqlServerDbContextOptions<T>(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Pobierz opcje budowania kontekstu serwera Sql
        /// Get the Sql Server context build options
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Opcje budowania kontekstu serwera Sql 
        /// Sql Server Context Build Options
        /// </returns>
        public static DbContextOptions GetDecryptSqlServerDbContextOptions<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder<T>();
                string connectionString = GetDecryptConnectionString(connectionStringName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    dbContextOptionsBuilder.UseSqlServer(connectionString);
                    return (DbContextOptions<T>)dbContextOptionsBuilder.Options;
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
        /// Pobierz opcje budowania kontekstu serwera Sql asynchronicznie
        /// Get the Sql Server context build options asynchronously
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Opcje budowania kontekstu serwera Sql 
        /// Sql Server Context Build Options
        /// </returns>
        public static async Task<DbContextOptions> GetDecryptSqlServerDbContextOptionsAsync<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => GetDecryptSqlServerDbContextOptions<T>(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Pobierz opcje budowania kontekstu serwera Sql
        /// Get the Sql Server context build options
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <param name="rsaFileName">
        /// Nazwa pliku szyfrującego
        /// The name of the encryption file
        /// </param>
        /// <returns>
        /// Opcje budowania kontekstu serwera Sql 
        /// Sql Server Context Build Options
        /// </returns>
        public static DbContextOptions GetDecryptSqlServerDbContextOptions<T>(string connectionStringName, string rsaFileName) where T : DbContext
        {
            try
            {
                DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder<T>();
                string connectionString = GetDecryptConnectionString(connectionStringName, rsaFileName);
                if (null != connectionString && !string.IsNullOrWhiteSpace(connectionString))
                {
                    dbContextOptionsBuilder.UseSqlServer(connectionString);
                    return (DbContextOptions<T>)dbContextOptionsBuilder.Options;
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
        /// Pobierz opcje budowania kontekstu serwera Sql asynchronicznie
        /// Get the Sql Server context build options asynchronously
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <param name="rsaFileName">
        /// Nazwa pliku szyfrującego
        /// The name of the encryption file
        /// </param>
        /// <returns>
        /// Opcje budowania kontekstu serwera Sql 
        /// Sql Server Context Build Options
        /// </returns>
        public static async Task<DbContextOptions> GetDecryptSqlServerDbContextOptionsAsync<T>(string connectionStringName, string rsaFileName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => GetDecryptSqlServerDbContextOptions<T>(connectionStringName, rsaFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Utwórz instancje do klasy kontekstowej bazy danych
        /// Create instances for the database context class
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <returns>
        /// Instancja do klasy kontekstowej bazy danych lub null
        /// An instance to the database context class or null
        /// </returns>
        public static T CreateInstancesForDatabaseContextClass<T>() where T : DbContext
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                return null;
            }
        }

        /// <summary>
        /// Utwórz instancje do klasy kontekstowej bazy danych asynchronicznie
        /// Create instances for the database context class asynchronously
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <returns>
        /// Instancja do klasy kontekstowej bazy danych lub null
        /// An instance to the database context class or null
        /// </returns>
        public static async Task<T> CreateInstancesForDatabaseContextClassAsync<T>() where T : DbContext
        {
            try
            {
                return await Task.Run(() => CreateInstancesForDatabaseContextClass<T>());
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Utwórz instancje do klasy kontekstowej bazy danych
        /// Create instances for the database context class
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Instancja do klasy kontekstowej bazy danych lub null
        /// An instance to the database context class or null
        /// </returns>
        public static T CreateInstancesForDatabaseContextClass<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                DbContextOptions dbContextOptions = GetSqlServerDbContextOptions<T>(connectionStringName);
                if (null != dbContextOptions)
                {
                    object[] vs = { GetSqlServerDbContextOptions<T>(connectionStringName) };
                    return (T)Activator.CreateInstance(typeof(T), vs);
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
        /// Utwórz instancje do klasy kontekstowej bazy danych asynchronicznie
        /// Create instances for the database context class asynchronously
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Instancja do klasy kontekstowej bazy danych lub null
        /// An instance to the database context class or null
        /// </returns>
        public static async Task<T> CreateInstancesForDatabaseContextClassAsync<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => CreateInstancesForDatabaseContextClass<T>(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Odszyfruj i utwórz instancje do klasy kontekstowej bazy danych
        /// Decrypt and create instances into the database context class
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Instancja do klasy kontekstowej bazy danych lub null
        /// An instance to the database context class or null
        /// </returns>
        public static T DecryptAndCreateInstancesForDatabaseContextClass<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                DbContextOptions dbContextOptions = GetDecryptSqlServerDbContextOptions<T>(connectionStringName);
                if (null != dbContextOptions)
                {
                    object[] vs = { GetDecryptSqlServerDbContextOptions<T>(connectionStringName) };
                    return (T)Activator.CreateInstance(typeof(T), vs);
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
        /// Odszyfruj i utwórz instancje do klasy kontekstowej bazy danych asynchronicznie
        /// Decrypt and create instances into the database context class asynchronously
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Instancja do klasy kontekstowej bazy danych lub null
        /// An instance to the database context class or null
        /// </returns>
        public static async Task<T> DecryptAndCreateInstancesForDatabaseContextClassAsync<T>(string connectionStringName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => DecryptAndCreateInstancesForDatabaseContextClass<T>(connectionStringName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }

        /// <summary>
        /// Odszyfruj i utwórz instancje do klasy kontekstowej bazy danych
        /// Decrypt and create instances into the database context class
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Instancja do klasy kontekstowej bazy danych lub null
        /// An instance to the database context class or null
        /// </returns>
        public static T DecryptAndCreateInstancesForDatabaseContextClass<T>(string connectionStringName, string rsaFileName) where T : DbContext
        {
            try
            {
                DbContextOptions dbContextOptions = GetDecryptSqlServerDbContextOptions<T>(connectionStringName, rsaFileName);
                if (null != dbContextOptions)
                {
                    object[] vs = { dbContextOptions };
                    return (T)Activator.CreateInstance(typeof(T), vs);
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
        /// Odszyfruj i utwórz instancje do klasy kontekstowej bazy danych asynchronicznie
        /// Decrypt and create instances into the database context class asynchronously
        /// </summary>
        /// <typeparam name="T">
        /// Typ kontekstu bazy danych
        /// Database context type
        /// </typeparam>
        /// <param name="connectionStringName">
        /// Nazwa ciągu połączenia String
        /// The name of the connection String
        /// </param>
        /// <returns>
        /// Instancja do klasy kontekstowej bazy danych lub null
        /// An instance to the database context class or null
        /// </returns>
        public static async Task<T> DecryptAndCreateInstancesForDatabaseContextClassAsync<T>(string connectionStringName, string rsaFileName) where T : DbContext
        {
            try
            {
                return await Task.Run(() => DecryptAndCreateInstancesForDatabaseContextClass<T>(connectionStringName, rsaFileName));
            }
            catch (Exception e)
            {
                await Task.Run(() => _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
                return null;
            }
        }
    }
}