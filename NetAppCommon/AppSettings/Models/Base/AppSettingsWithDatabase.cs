#region using

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.Serialization;
using log4net;
using Microsoft.EntityFrameworkCore;
using NetAppCommon.Validation;
using Newtonsoft.Json;

#endregion

#nullable enable annotations

namespace NetAppCommon.AppSettings.Models.Base
{
    #region public virtual class AppSettings

    /// <summary>
    ///     Wspólny model danych ustawień aplikacji
    ///     Common application settings data model
    /// </summary>
    [NotMapped]
    public class AppSettingsWithDatabase : AppSettingsWithoutDatabase
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Instancja do klasy Log4netLogger
        ///     Instance to Log4netLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        public AppSettingsWithDatabase()
        {
        }

        public AppSettingsWithDatabase(string filePath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    base.FilePath = filePath;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region public virtual string GetConnectionStringName()

        /// <summary>
        ///     Pobierz nazwę połączenia bazy danych Mssql (domyślną)
        ///     Get the Mssql database connection name (default)
        /// </summary>
        /// <returns>
        ///     Nazwa połączenia bazy danych Mssql (domyślna)
        ///     Mssql database connection name (default)
        /// </returns>
        public virtual string GetConnectionStringName()
        {
            return ConnectionStringName!;
        }

        #endregion

        #region public virtual string GetConnectionString()

        /// <summary>
        ///     Pobierz parametry połączenia
        ///     Get the connection string
        /// </summary>
        /// <returns>
        ///     Parametry połączenia jako string lub null
        ///     Connection string as string or null
        /// </returns>
        public virtual string GetConnectionString()
        {
            var connectionString = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(ConnectionString))
                {
                    connectionString = AesIVProviderService.Decpypt(ConnectionString,
                        RsaProviderService.AsymmetricPublicKeyAsString);
                    connectionString = RsaProviderService.DecryptWithPublicKey(connectionString);
                    connectionString = DatabaseMssql.ParseConnectionString(connectionString);
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return connectionString;
        }

        #endregion

        #region public virtual DbContextOptionsBuilder GetDbContextOptionsBuilder<TContext>() where TContext : DbContext

        /// <summary>
        ///     public virtual DbContextOptionsBuilder GetDbContextOptionsBuilder
        ///     public virtual DbContextOptionsBuilder GetDbContextOptionsBuilder
        /// </summary>
        /// <typeparam name="TContext">
        ///     TContext : DbContext
        ///     TContext : DbContext
        /// </typeparam>
        /// <returns>
        ///     DbContextOptionsBuilder
        ///     DbContextOptionsBuilder
        /// </returns>
        public virtual DbContextOptionsBuilder GetDbContextOptionsBuilder<TContext>() where TContext : DbContext
        {
            try
            {
                if (null == DbContextOptionsBuilder && !string.IsNullOrWhiteSpace(ConnectionString))
                {
                    DbContextOptionsBuilder = new DbContextOptionsBuilder<TContext>();
                    DbContextOptionsBuilder.UseSqlServer(GetConnectionString());
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return ((DbContextOptionsBuilder<TContext>) DbContextOptionsBuilder)!;
        }

        #endregion

        #region public virtual DbContextOptions<TContext> GetDbContextOptions<TContext>() where TContext : DbContext

        /// <summary>
        ///     GetDbContextOptions
        ///     GetDbContextOptions
        /// </summary>
        /// <typeparam name="TContext">
        ///     TContext : DbContext
        ///     TContext : DbContext
        /// </typeparam>
        /// <returns>
        ///     DbContextOptions
        ///     DbContextOptions
        /// </returns>
        public virtual DbContextOptions<TContext> GetDbContextOptions<TContext>() where TContext : DbContext
        {
            try
            {
                if (null == DbContextOptions && null != ConnectionString)
                {
                    DbContextOptionsBuilder = GetDbContextOptionsBuilder<TContext>();
                    DbContextOptions = DbContextOptionsBuilder.Options;
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return ((DbContextOptions<TContext>) DbContextOptions)!;
        }

        #endregion

        #region private bool _checkAndMigrate; public bool CheckAndMigrate

        private bool _checkAndMigrate;

        /// <summary>
        ///     Wymuś instalację i aktualizację bazy danych
        ///     Force database installation and update
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [Display(Name = "Wymuś instalację i aktualizację bazy danych",
            Prompt = "Zaznacz dla wykonania wymuś instalację i aktualizację bazy danych",
            Description = "Wymuś instalację i aktualizację bazy danych")]
        public virtual bool CheckAndMigrate
        {
            get => _checkAndMigrate;
            set
            {
                if (value != _checkAndMigrate)
                {
                    _checkAndMigrate = value;
                    OnPropertyChanged(nameof(CheckAndMigrate));
                }
            }
        }

        #endregion

        #region public DateTime LastMigrateDateTime { get; private set; }

        private DateTime? _lastMigrateDateTime;

        /// <summary>
        ///     Data ostatniej próby aktualizacji migracji bazy danych
        ///     Date of the last database migration update attempt
        /// </summary>
        [JsonProperty(nameof(LastMigrateDateTime))]
        [Display(Name = "Data ostatniej próby aktualizacji migracji bazy danych",
            Prompt = "Wpisz lub wybierz datę ostatniej próby aktualizacji migracji bazy danych",
            Description = "Data ostatniej próby aktualizacji migracji bazy danych")]
        public DateTime? LastMigrateDateTime
        {
            get
            {
                if (null == _lastMigrateDateTime)
                {
                    _lastMigrateDateTime = AppSettingsRepository?.GetValue<DateTime>(this, "LastMigrateDateTime");
                }

                return _lastMigrateDateTime;
            }
            set
            {
                if (value != _lastMigrateDateTime)
                {
                    _lastMigrateDateTime = value;
                    OnPropertyChanged(nameof(LastMigrateDateTime));
                }
            }
        }

        #endregion

        #region private int _cacheLifeTime; int CacheLifeTime

        private int? _cacheLifeTime;

        /// <summary>
        ///     Okres istnienia pamięci podręcznej (w sekundach)
        ///     Cache lifetime (in seconds)
        /// </summary>
        [JsonProperty(nameof(CacheLifeTime))]
        [Display(Name = "Okres istnienia pamięci podręcznej (w sekundach)",
            Prompt = "Wpisz okres istnienia pamięci podręcznej (w sekundach)",
            Description = "Okres istnienia pamięci podręcznej (w sekundach)")]
        [Range(0, 2147483647)]
        [Required]
        public int? CacheLifeTime
        {
            get
            {
                try
                {
                    if (null == _cacheLifeTime)
                    {
                        _cacheLifeTime = AppSettingsRepository?.GetValue<int>(this, "CacheLifeTime");
                    }

                    return _cacheLifeTime;
                }
                catch (Exception e)
                {
                    _log4Net.Error(
                        $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                }

                return _cacheLifeTime;
            }
            set
            {
                if (value != _cacheLifeTime)
                {
                    _cacheLifeTime = value;
                    OnPropertyChanged(nameof(CacheLifeTime));
                }
            }
        }

        #endregion

        #region private bool _useGlobalDatabaseConnectionSettings; public bool UseGlobalDatabaseConnectionSettings

        private bool? _useGlobalDatabaseConnectionSettings;

        /// <summary>
        ///     Sprawdź możliwość podłączenia do bazy danych\nz wpisanego parametru Ciąg połączenia do bazy danych Mssql
        ///     Check the possibility of connecting to the database by entering the Mssql database connection string parameter
        /// </summary>
        [Display(Name = "Używaj globalnych ustawień połączenia bazy danych z pliku ustawień appsettings.json",
            Prompt =
                "Zaznacz, jeśli chcesz używać globalnych ustawień połączenia bazy danych z pliku ustawień appsettings.json",
            Description = "Używaj globalnych ustawień połączenia bazy danych z pliku ustawień appsettings.json")]
        public bool UseGlobalDatabaseConnectionSettings
        {
            get
            {
                try
                {
                    _useGlobalDatabaseConnectionSettings ??=
                        AppSettingsRepository?.GetValue<bool>(this, "UseGlobalDatabaseConnectionSettings");
                }
                catch (Exception e)
                {
                    _log4Net.Error(
                        $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                }

                return _useGlobalDatabaseConnectionSettings != null && (bool) _useGlobalDatabaseConnectionSettings;
            }
            set
            {
                if (value != _useGlobalDatabaseConnectionSettings)
                {
                    _useGlobalDatabaseConnectionSettings = value;
                    OnPropertyChanged(nameof(UseGlobalDatabaseConnectionSettings));
                }
            }
        }

        #endregion

        #region private bool _checkForConnection; public bool CheckForConnection

        private bool _checkForConnection;

        /// <summary>
        ///     Sprawdź możliwość podłączenia do bazy danych\nz wpisanego parametru Ciąg połączenia do bazy danych Mssql
        ///     Check the possibility of connecting to the database by entering the Mssql database connection string parameter
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [Display(
            Name =
                "Sprawdź możliwość podłączenia do bazy danych\nz wpisanego parametru Ciąg połączenia do bazy danych Mssql",
            Prompt =
                "Zaznacz, jeśli chcesz sprawdzić możliwość podłączenia do bazy danych z wpisanego parametru Ciąg połączenia do bazy danych Mssql",
            Description =
                "Sprawdź możliwość podłączenia do bazy danych\nz wpisanego parametru Ciąg połączenia do bazy danych Mssql")]
        public bool CheckForConnection
        {
            get => _checkForConnection;
            set
            {
                if (value != _checkForConnection)
                {
                    _checkForConnection = value;
                    OnPropertyChanged(nameof(CheckForConnection));
                }
            }
        }

        #endregion

        #region private string _connectionStringName; public virtual string ConnectionStringName

        private string? _connectionStringName;

        /// <summary>
        ///     Nazwa połączenia bazy danych Mssql dla bieżącej aplikacji
        ///     The name of the Mssql database connection for the current application
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual string? ConnectionStringName
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

        #region private string _connectionString; public virtual string ConnectionString

        private string? _connectionString;

        /// <summary>
        ///     Ciąg połączenia do bazy danych Mssql jako string
        ///     Mssql database connection string as a string
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Ciąg połączenia do bazy danych Mssql", Prompt = "Wpisz ciąg połączenia do bazy danych Mssql",
            Description = "Ciąg połączenia do bazy danych Mssql")]
        [Required]
        [StringLength(1024)]
        [MssqlCanConnect]
        public virtual string? ConnectionString
        {
            get
            {
                if (null == _connectionString)
                {
                    try
                    {
                        _connectionString = AppSettingsRepository?.GetValue<string>(this,
                            $"{nameof(ConnectionStrings)}:{ConnectionStringName}");
                    }
                    catch (Exception e)
                    {
                        _log4Net.Error(
                            $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                    }
                    finally
                    {
                        if (null != _connectionString && _connectionString.Length <= 512)
                        {
                            _connectionString = RsaProviderService.EncryptWithPrivateKey(_connectionString);
                            _connectionString = AesIVProviderService.Encrypt(_connectionString,
                                RsaProviderService.AsymmetricPublicKeyAsString);
                        }
                    }
                }

                return _connectionString;
            }
            set
            {
                if (value != _connectionString)
                {
                    if (null != value && value.Length <= 512)
                    {
                        value = RsaProviderService.EncryptWithPrivateKey(value);
                        value = AesIVProviderService.Encrypt(value, RsaProviderService.AsymmetricPublicKeyAsString);
                    }

                    _connectionString = value;
                    OnPropertyChanged(nameof(ConnectionString));
                    if (!string.IsNullOrWhiteSpace(ConnectionStringName) &&
                        null != _connectionString && !string.IsNullOrWhiteSpace(_connectionString))
                    {
                        ConnectionStrings = new Dictionary<string, string> {{ConnectionStringName, _connectionString}};
                    }
                }
            }
        }

        #endregion

        #region private Dictionary<string, string> _connectionStrings; public virtual Dictionary<string, string> ConnectionStrings

        private Dictionary<string, string>? _connectionStrings;

        /// <summary>
        ///     Słownik zawierający definicję połączenia z nazwą klucza konfiguracji i wartością jako Dictionary
        ///     A dictionary containing a connection definition with a configuration key name and value as Dictionary
        /// </summary>
        [JsonProperty(nameof(ConnectionStrings))]
        public virtual Dictionary<string, string>? ConnectionStrings
        {
            get
            {
                if (null != ConnectionStringName && !string.IsNullOrWhiteSpace(ConnectionStringName) &&
                    null != ConnectionString && !string.IsNullOrWhiteSpace(ConnectionString))
                {
                    _connectionStrings = new Dictionary<string, string> {{ConnectionStringName, ConnectionString}};
                }

                return _connectionStrings;
            }
            private set
            {
                if (value != _connectionStrings)
                {
                    _connectionStrings = value;
                    OnPropertyChanged(nameof(ConnectionStrings));
                }
            }
        }

        #endregion

        #region private DbContextOptionsBuilder _dbContextOptionsBuilder; public virtual DbContextOptionsBuilder DbContextOptionsBuilder

        private DbContextOptionsBuilder? _dbContextOptionsBuilder;

        /// <summary>
        ///     DbContextOptionsBuilder DbContextOptionsBuilder
        ///     DbContextOptionsBuilder DbContextOptionsBuilder
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [NotRequired]
        public virtual DbContextOptionsBuilder? DbContextOptionsBuilder
        {
            get => _dbContextOptionsBuilder;
            set
            {
                if (!Equals(value, _dbContextOptionsBuilder))
                {
                    _dbContextOptionsBuilder = value;
                    OnPropertyChanged(nameof(DbContextOptionsBuilder));
                }
            }
        }

        #endregion

        #region private DbContextOptions _dbContextOptions; public virtual DbContextOptions DbContextOptions

        private DbContextOptions? _dbContextOptions;

        /// <summary>
        ///     public virtual DbContextOptions DbContextOptions
        ///     public virtual DbContextOptions DbContextOptions
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public virtual DbContextOptions? DbContextOptions
        {
            get => _dbContextOptions;
            set
            {
                if (value != _dbContextOptions)
                {
                    _dbContextOptions = value;
                    OnPropertyChanged(nameof(DbContextOptions));
                }
            }
        }

        #endregion
    }

    #endregion
}