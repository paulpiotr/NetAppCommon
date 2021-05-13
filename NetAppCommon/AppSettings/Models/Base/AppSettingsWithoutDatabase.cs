#region using

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using log4net;
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
    public class AppSettingsWithoutDatabase : RsaProvaiderBaseModel, INotifyPropertyChanged
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Instancja do klasy Log4netLogger
        ///     Instance to Log4netLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public AppSettingsWithoutDatabase()

        public AppSettingsWithoutDatabase()
        {
        }

        #endregion

        #region public AppSettingsWithoutDatabase(string filePath)

        /// <summary>
        ///     Konstruktor
        ///     Constructor
        /// </summary>
        /// <param name="filePath">
        ///     Ścieżka do pliku ustawień jako string
        ///     Path to the settings file as a string
        /// </param>
        public AppSettingsWithoutDatabase(string filePath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    _filePath = filePath;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     PropertyChangedEventHandler PropertyChanged
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region public virtual AppSettingsWithoutDatabase Setup(string filePath = null!)

        /// <summary>
        ///     Zainicjuj i ustaw
        ///     Initialize and set up
        /// </summary>
        /// <param name="filePath">
        ///     Ścieżka do pliku ustawień jako string
        ///     Path to the settings file as a string
        /// </param>
        /// <returns>
        ///     this
        ///     this
        /// </returns>
        protected virtual AppSettingsWithoutDatabase Setup(string filePath = null!)
        {
            return this;
        }

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
        public virtual string GetFileName()
        {
            return FileName!;
        }

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
        public virtual string GetFilePath()
        {
            return FilePath!;
        }

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
        public IConfigurationBuilder GetConfigurationBuilder()
        {
            return AppSettingsConfigurationBuilder!;
        }

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
        public IConfigurationRoot GetConfigurationRoot()
        {
            return AppSettingsConfigurationRoot!;
        }

        #endregion

        #region private virtual void OnPropertyChanged(PropertyChangedEventArgs args)

        /// <summary>
        ///     private virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        #endregion

        #region public void OnPropertyChanged(string propertyName)

        /// <summary>
        ///     public void OnPropertyChanged(string propertyName)
        ///     public void OnPropertyChanged(string propertyName)
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        public void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region private AppSettingsRepositoryBase...; public virtual AppSettingsRepositoryBase...

        private AppSettingsRepository<AppSettingsWithoutDatabase>? _appSettingsRepository;

        [XmlIgnore]
        [JsonIgnore]
        public virtual AppSettingsRepository<AppSettingsWithoutDatabase>? AppSettingsRepository
        {
            get =>
                _appSettingsRepository ??= AppSettingsRepository<AppSettingsWithoutDatabase>.GetInstance();
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

        #region private AesIVProviderService...; public AesIVProviderService...

        private AesIVProviderService? _aesIVProviderService;

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

        #region private RsaProviderService...; public RsaProviderService...

        private RsaProviderService? _rsaProviderService;

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
                        AsymmetricprivateKeyFilePath,
                        AsymmetricpublicKeyFilePath,
                        !File.Exists(AsymmetricprivateKeyFilePath) || !File.Exists(AsymmetricpublicKeyFilePath)
                    );
                    _rsaProviderService.SaveAsymmetricKeyPairToFile(AsymmetricprivateKeyFilePath,
                        AsymmetricpublicKeyFilePath, true);
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

        #region private new string _asymmetricprivateKeyFilePath; public new string AsymmetricprivateKeyFilePath

        private string? _asymmetricprivateKeyFilePath;

        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public string AsymmetricprivateKeyFilePath
        {
            get =>
                _asymmetricprivateKeyFilePath ??= Path.Combine(UserProfileDirectory!, ".ssh", "id_rsa");
            set
            {
                if (value != _asymmetricprivateKeyFilePath)
                {
                    _asymmetricprivateKeyFilePath = value;
                }
            }
        }

        #endregion

        #region private new string _asymmetricpublicKeyFilePath; public new string AsymmetricpublicKeyFilePath

        private string? _asymmetricpublicKeyFilePath;

        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public string AsymmetricpublicKeyFilePath
        {
            get =>
                _asymmetricpublicKeyFilePath ??= Path.Combine(UserProfileDirectory!, ".ssh", "id_rsa.pub");
            set
            {
                if (value != _asymmetricpublicKeyFilePath)
                {
                    _asymmetricpublicKeyFilePath = value;
                }
            }
        }

        #endregion

        #region private string _baseDirectory; public virtual string BaseDirectory

        private string? _baseDirectory;

        /// <summary>
        ///     public virtual string BaseDirectory
        ///     public virtual string BaseDirectory
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Główny katalog programu",
            Prompt = "Wpisz lub wybierz główny katalog programu",
            Description = "Główny katalog programu")]
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
            private set
            {
                if (value != _baseDirectory && Directory.Exists(value))
                {
                    _baseDirectory = value;
                    OnPropertyChanged(nameof(BaseDirectory));
                }
            }
        }

        #endregion

        #region private string _userProfileDirectory; public virtual string UserProfileDirectory

        private string? _userProfileDirectory;

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
                            _userProfileDirectory = Path.Combine(_userProfileDirectory,
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

        #region private string _fileName; public virtual string FileName

        private string? _fileName;

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

        #region private string _setupFileName; public virtual string SetupFileName

        private string? _setupFileName;

        /// <summary>
        ///     Nazwa pliku z ustawieniami startowymi aplikacji
        ///     The name of the application startup settings file
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual string? SetupFileName
        {
            get => _setupFileName;
            protected set
            {
                if (value != _setupFileName)
                {
                    _setupFileName = value;
                    OnPropertyChanged("SetupFileName");
                }
            }
        }

        #endregion

        #region private string _filePath; public virtual string FilePath

        private string? _filePath;

        /// <summary>
        ///     Absolutna ścieżka do pliku konfiguracji
        ///     The absolute path to the configuration file
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Absolutna ścieżka do pliku konfiguracji ustawień programu",
            Prompt = "Wybierz lub wpisz absolutną ścieżkę do pliku konfiguracji ustawień programu",
            Description = "Absolutna ścieżka do pliku konfiguracji ustawień programu")]
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

        #region private IConfigurationBuilder _appSettingsConfigurationBuilder; public virtual IConfigurationBuilder AppSettingsConfigurationBuilder

        private IConfigurationBuilder? _appSettingsConfigurationBuilder;

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

        #region private IConfigurationRoot _appSettingsConfigurationRoot; public virtual IConfigurationRoot AppSettingsConfigurationRoot

        private IConfigurationRoot? _appSettingsConfigurationRoot;

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
    }

    #endregion
}