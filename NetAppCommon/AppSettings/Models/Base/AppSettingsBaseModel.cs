#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetAppCommon.AppSettings.Repositories.Base;
using NetAppCommon.Crypto.AesCryptography.Services;
using NetAppCommon.Crypto.BouncyCastle.Models.Base;
using NetAppCommon.Crypto.BouncyCastle.Services;
using NetAppCommon.Validation;
using Newtonsoft.Json;
using static System.Environment;

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
    public class AppSettingsBaseModel : RsaProvaiderBaseModel
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Instancja do klasy Log4netLogger
        ///     Instance to Log4netLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public virtual string GetFileName()

        /// <summary>
        ///     Pobierz nazwę pliku z ustawieniami aplikacji
        ///     Get the name of the application settings file
        /// </summary>
        /// <returns>
        ///     Nazwa pliku z ustawieniami aplikacji jako string
        ///     Name of the application settings file as a string
        /// </returns>
        public virtual string GetFileName() => FileName;

        #endregion

        #region public virtual string GetFilePath()

        /// <summary>
        ///     Pobierz bieżącą ścieżkę do pliku konfiguracji
        ///     Get the current path to the configuration file
        /// </summary>
        /// <returns>
        ///     Bieżąca ścieżka do pliku konfiguracji jako string
        ///     Current path to the configuration file as a string
        /// </returns>
        public virtual string GetFilePath() => FilePath;

        #endregion

        #region public IConfigurationBuilder GetConfigurationBuilder()

        /// <summary>
        ///     Pobierz  IConfigurationBuilder ConfigurationBuilder
        ///     Get IConfigurationBuilder ConfigurationBuilder
        /// </summary>
        /// <returns>
        ///     Instancja IConfigurationBuilder ConfigurationBuilder
        ///     IConfigurationBuilder ConfigurationBuilder instance
        /// </returns>
        public IConfigurationBuilder GetConfigurationBuilder() => AppSettingsConfigurationBuilder;

        #endregion

        #region public IConfigurationRoot GetConfigurationRoot()

        /// <summary>
        ///     public IConfigurationRoot GetConfigurationRoot()
        ///     public IConfigurationRoot GetConfigurationRoot()
        /// </summary>
        /// <returns>
        ///     public IConfigurationRoot AppSettingsConfigurationRoot
        ///     public IConfigurationRoot AppSettingsConfigurationRoot
        /// </returns>
        public IConfigurationRoot GetConfigurationRoot() => AppSettingsConfigurationRoot;

        #endregion

        #region public virtual string GetConnectionStringName()

        /// <summary>
        ///     Pobierz nazwę połączenia bazy danych Mssql (domyślną)
        ///     Get the Mssql database connection name (default)
        /// </summary>
        /// <returns>
        ///     Nazwa połączenia bazy danych Mssql (domyślna)
        ///     Mssql database connection name (default)
        /// </returns>
        public virtual string GetConnectionStringName() => ConnectionStringName;

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

            return (DbContextOptionsBuilder<TContext>)DbContextOptionsBuilder;
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
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }

            return (DbContextOptions<TContext>)DbContextOptions;
        }

        #endregion

        #region public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     PropertyChangedEventHandler PropertyChanged
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)

        /// <summary>
        ///     protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        #endregion

        #region protected void OnPropertyChanged(string propertyName)

        /// <summary>
        ///     protected void OnPropertyChanged(string propertyName)
        ///     protected void OnPropertyChanged(string propertyName)
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        protected void OnPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        #endregion

        #region protected AppSettingsRepositoryBase...; public virtual AppSettingsRepositoryBase...

        protected AppSettingsRepositoryBase<AppSettingsBaseModel>? _appSettingsRepository;

        [XmlIgnore]
        [JsonIgnore]
        public virtual AppSettingsRepositoryBase<AppSettingsBaseModel>? AppSettingsRepository
        {
            get =>
                _appSettingsRepository ??= AppSettingsRepositoryBase<AppSettingsBaseModel>.GetInstance();
            set
            {
                if (value != _appSettingsRepository)
                {
                    _appSettingsRepository = value;
                    OnPropertyChanged(nameof(AppSettingsRepository));
                }
            }
        }

        #endregion

        #region protected AesIVProviderService...; public AesIVProviderService...

        protected AesIVProviderService? _aesIVProviderService;

        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public AesIVProviderService AesIVProviderService
        {
            get => _aesIVProviderService ??= new AesIVProviderService();
            set
            {
                if (value != _aesIVProviderService)
                {
                    _aesIVProviderService = value;
                    OnPropertyChanged(nameof(AesIVProviderService));
                }
            }
        }

        #endregion

        #region protected RsaProviderService...; public RsaProviderService...

        protected RsaProviderService? _rsaProviderService;

        /// <summary>
        ///     public RsaProviderService RsaProviderService
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public RsaProviderService RsaProviderService
        {
            get
            {
                if (null == _rsaProviderService)
                {
                    _rsaProviderService = new RsaProviderService(
                        AsymmetricPrivateKeyFilePath,
                        AsymmetricPublicKeyFilePath,
                        !File.Exists(AsymmetricPrivateKeyFilePath) || !File.Exists(AsymmetricPublicKeyFilePath)
                    );
                    _rsaProviderService.SaveAsymmetricKeyPairToFile(AsymmetricPrivateKeyFilePath,
                        AsymmetricPublicKeyFilePath, true);
                }

                return _rsaProviderService;
            }
            set
            {
                if (value != _rsaProviderService)
                {
                    _rsaProviderService = value;
                    OnPropertyChanged(nameof(RsaProviderService));
                }
            }
        }

        #endregion

        #region protected new string _asymmetricPrivateKeyFilePath; public new string AsymmetricPrivateKeyFilePath

        protected new string? _asymmetricPrivateKeyFilePath;

        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public new string AsymmetricPrivateKeyFilePath
        {
            get =>
                _asymmetricPrivateKeyFilePath ??= Path.Combine(UserProfileDirectory, ".ssh", "id_rsa");
            set
            {
                if (value != _asymmetricPrivateKeyFilePath)
                {
                    _asymmetricPrivateKeyFilePath = value;
                }
            }
        }

        #endregion

        #region protected new string _asymmetricPublicKeyFilePath; public new string AsymmetricPublicKeyFilePath

        protected new string? _asymmetricPublicKeyFilePath;

        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public new string AsymmetricPublicKeyFilePath
        {
            get =>
                _asymmetricPublicKeyFilePath ??= Path.Combine(UserProfileDirectory, ".ssh", "id_rsa.pub");
            set
            {
                if (value != _asymmetricPublicKeyFilePath)
                {
                    _asymmetricPublicKeyFilePath = value;
                }
            }
        }

        #endregion

        #region protected string _baseDirectory; public virtual string BaseDirectory

        protected string? _baseDirectory;

        /// <summary>
        ///     public virtual string BaseDirectory
        ///     public virtual string BaseDirectory
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual string? BaseDirectory
        {
            get
            {
                try
                {
                    _baseDirectory ??= AppDomain.CurrentDomain.BaseDirectory;
                }
                catch (Exception e)
                {
                    _log4Net.Error(
                        $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                }

                return _baseDirectory ?? string.Empty;
            }
            protected set
            {
                if (value != _baseDirectory && Directory.Exists(value))
                {
                    _baseDirectory = value;
                    OnPropertyChanged(nameof(BaseDirectory));
                }
            }
        }

        #endregion

        #region protected string _userProfileDirectory; public virtual string UserProfileDirectory

        protected string? _userProfileDirectory;

        /// <summary>
        ///     public virtual string UserProfileDirectory
        ///     public virtual string UserProfileDirectory
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual string? UserProfileDirectory
        {
            get
            {
                try
                {
                    if (null == _userProfileDirectory)
                    {
                        _userProfileDirectory = GetFolderPath(SpecialFolder.UserProfile);
                        if (Directory.Exists(_userProfileDirectory))
                        {
                            _userProfileDirectory = Path.Combine(GetFolderPath(SpecialFolder.UserProfile),
                                Assembly.GetExecutingAssembly().GetName().Name);
                            if (!Directory.Exists(_userProfileDirectory))
                            {
                                Directory.CreateDirectory(_userProfileDirectory);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _userProfileDirectory = BaseDirectory;
                    _log4Net.Error(
                        $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                }

                return _userProfileDirectory;
            }
            protected set
            {
                if (value != _userProfileDirectory && Directory.Exists(value))
                {
                    _userProfileDirectory = value;
                    OnPropertyChanged(nameof(UserProfileDirectory));
                }
            }
        }

        #endregion

        #region protected string _fileName; public virtual string FileName

        protected string? _fileName;

        /// <summary>
        ///     Nazwa pliku z ustawieniami aplikacji ustawiona w zależności od wersji środowiska
        ///     Application settings file name set depending on the version of the environment
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual string? FileName
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

        #region protected string _filePath; public virtual string FilePath

        protected string? _filePath;

        /// <summary>
        ///     Absolutna ścieżka do pliku konfiguracji
        ///     The absolute path to the configuration file
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual string? FilePath
        {
            get
            {
                _filePath ??= Path.Combine(BaseDirectory ?? string.Empty, FileName ?? string.Empty);
                return null != _filePath && File.Exists(_filePath) ? _filePath : null;
            }
            set
            {
                if (value != _filePath)
                {
                    _filePath = value;
                    OnPropertyChanged("FilePath");
                }
            }
        }

        #endregion

        #region protected IConfigurationBuilder _appSettingsConfigurationBuilder; public virtual IConfigurationBuilder AppSettingsConfigurationBuilder

        protected IConfigurationBuilder? _appSettingsConfigurationBuilder;

        /// <summary>
        ///     IConfigurationBuilder ConfigurationBuilder
        ///     IConfigurationBuilder ConfigurationBuilder
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public virtual IConfigurationBuilder? AppSettingsConfigurationBuilder
        {
            get
            {
                if (null == _appSettingsConfigurationBuilder &&
                    !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
                {
                    _appSettingsConfigurationBuilder = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(FilePath))
                        .AddJsonFile(Path.GetFileName(FilePath), true, true);
                }

                return _appSettingsConfigurationBuilder!;
            }
            set
            {
                if (value != _appSettingsConfigurationBuilder)
                {
                    _appSettingsConfigurationBuilder = value;
                    OnPropertyChanged(nameof(AppSettingsConfigurationBuilder));
                }
            }
        }

        #endregion

        #region protected IConfigurationRoot _appSettingsConfigurationRoot; public virtual IConfigurationRoot AppSettingsConfigurationRoot

        protected IConfigurationRoot? _appSettingsConfigurationRoot;

        /// <summary>
        ///     public IConfigurationRoot AppSettingsConfigurationRoot
        ///     public IConfigurationRoot AppSettingsConfigurationRoot
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual IConfigurationRoot? AppSettingsConfigurationRoot
        {
            get
            {
                if (null == _appSettingsConfigurationRoot && !string.IsNullOrWhiteSpace(FilePath) &&
                    File.Exists(FilePath))
                {
                    _appSettingsConfigurationRoot = AppSettingsConfigurationBuilder?.Build();
                }

                return _appSettingsConfigurationRoot!;
            }
            set
            {
                if (value != _appSettingsConfigurationRoot)
                {
                    _appSettingsConfigurationRoot = value;
                    OnPropertyChanged(nameof(AppSettingsConfigurationRoot));
                }
            }
        }

        #endregion

        #region protected bool _checkAndMigrate; public bool CheckAndMigrate

        protected bool _checkAndMigrate;

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

        #region public DateTime LastMigrateDateTime { get; protected set; }

        protected DateTime? _lastMigrateDateTime;

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
                    _lastMigrateDateTime = AppSettingsRepository.GetValue<DateTime>(this, "LastMigrateDateTime");
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

        #region protected int _cacheLifeTime; int CacheLifeTime

        protected int? _cacheLifeTime;

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
                        _cacheLifeTime = AppSettingsRepository.GetValue<int>(this, "CacheLifeTime");
                    }

                    return _cacheLifeTime;
                }
                catch (Exception e)
                {
                    _log4Net.Error(
                        string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                            e.StackTrace), e);
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

        #region protected bool _useGlobalDatabaseConnectionSettings; public bool UseGlobalDatabaseConnectionSettings

        protected bool? _useGlobalDatabaseConnectionSettings;

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
                        AppSettingsRepository.GetValue<bool>(this, "UseGlobalDatabaseConnectionSettings");
                }
                catch (Exception e)
                {
                    _log4Net.Error(
                        string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                            e.StackTrace), e);
                }

                return _useGlobalDatabaseConnectionSettings != null && (bool)_useGlobalDatabaseConnectionSettings;
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

        #region protected bool _checkForConnection; public bool CheckForConnection

        protected bool _checkForConnection;

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

        #region protected string _connectionStringName; public virtual string ConnectionStringName

        protected string? _connectionStringName;

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

        #region protected string _connectionString; public virtual string ConnectionString

        protected string? _connectionString;

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
                        //if (string.IsNullOrWhiteSpace(_connectionString))
                        //{
                        //    _connectionString =
                        //        @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename=%Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)%\MSSQLLocalDB\MSSQLLocalDB.mdf; Database=%AttachDbFilename%; MultipleActiveResultSets=true; Integrated Security=SSPI; Trusted_Connection=Yes; Max Pool Size=65536; Pooling=True";
                        //}
                    }
                    catch (Exception e)
                    {
                        //_connectionString =
                        //    @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename=%Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)%\MSSQLLocalDB\MSSQLLocalDB.mdf; Database=%AttachDbFilename%; MultipleActiveResultSets=true; Integrated Security=SSPI; Trusted_Connection=Yes; Max Pool Size=65536; Pooling=True";
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

        #region protected Dictionary<string, string> _connectionStrings; public virtual Dictionary<string, string> ConnectionStrings

        protected Dictionary<string, string>? _connectionStrings;

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
            protected set
            {
                if (value != _connectionStrings)
                {
                    _connectionStrings = value;
                    OnPropertyChanged(nameof(ConnectionStrings));
                }
            }
        }

        #endregion

        #region protected DbContextOptionsBuilder _dbContextOptionsBuilder; public virtual DbContextOptionsBuilder DbContextOptionsBuilder

        protected DbContextOptionsBuilder? _dbContextOptionsBuilder;

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

        #region protected DbContextOptions _dbContextOptions; public virtual DbContextOptions DbContextOptions

        protected DbContextOptions? _dbContextOptions;

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
