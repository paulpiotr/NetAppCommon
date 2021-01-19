using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NetAppCommon.Crypto.BouncyCastle.Services.Interface;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace NetAppCommon.Crypto.BouncyCastle.Services
{
    public class RsaProviderService : Models.Base.RsaProvaiderBaseModel, IRsaProviderService
    {
        #region private readonly log4net.ILog _log4net
        /// <summary>
        /// Referencja klasy Log4NetLogget
        /// Reference to the Log4NetLogget class
        /// </summary>
        private readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region private const int DwKeySize = 4096;
        /// <summary>
        /// Rozmiar klucza - domyślna wartość jako int
        /// Key size - default value as int
        /// </summary>
        private const int DwKeySize = 4096;
        #endregion

        #region public RsaProviderService()
        /// <summary>
        /// Konstruktor
        /// Constructor
        /// </summary>
        public RsaProviderService()
        {
            Initialize(4096);
        }
        #endregion

        #region public static RsaProviderService GetRsaProviderService()
        /// <summary>
        /// Pobierz statyczną referencję klasy RsaProviderService
        /// Get a static RsaProviderService class reference
        /// </summary>
        /// <returns>
        /// Statyczna referencja jako RsaProviderService
        /// Static reference as Rsa Provider Service
        /// </returns>
        public static RsaProviderService GetRsaProviderService()
        {
            return new RsaProviderService();
        }
        #endregion

        #region public RsaProviderService(int dwKeySize = DwKeySize)
        /// <summary>
        /// Konstruktor
        /// Constructor
        /// </summary>
        /// <param name="dwKeySize">
        /// Rozmiar klucza jako int
        /// Key size as int
        /// </param>
        public RsaProviderService(int dwKeySize = DwKeySize)
        {
            Initialize(dwKeySize);
        }
        #endregion

        public static RsaProviderService GetRsaProviderService(int dwKeySize = DwKeySize)
        {
            return new RsaProviderService(dwKeySize);
        }

        public RsaProviderService(string asymmetricPrivateKeyFilePath, string asymmetricPublicKeyFilePath, bool initialize = true, int dwKeySize = DwKeySize)
        {
            if (initialize)
            {
                Initialize(dwKeySize);
                //Console.WriteLine("Initialize(dwKeySize);");
            }
            AsymmetricPrivateKeyFilePath = asymmetricPrivateKeyFilePath;
            AsymmetricPublicKeyFilePath = asymmetricPublicKeyFilePath;
        }

        public static RsaProviderService GetRsaProviderService(string asymmetricPrivateKeyFilePath, string asymmetricPublicKeyFilePath, bool initialize = true, int dwKeySize = DwKeySize)
        {
            return new RsaProviderService(asymmetricPrivateKeyFilePath, asymmetricPublicKeyFilePath, initialize, dwKeySize);
        }

        #region public virtual RsaServiceProvider Initialize(int dwKeySize = DwKeySize)
        /// <summary>
        /// Inicjuj
        /// Initialize
        /// <para>
        /// Inicjuje RsaKeyPairGenerator przez wywołanie new RsaKeyPairGenerator(); co umożliwia stosowanie instancji bez ręcznego ustawiania parametrów
        /// Initializes the RsaKeyPairGenerator by calling new RsaKeyPairGenerator (); which enables the use of instances without manual parameter setting
        /// </para>
        /// </summary>
        /// <param name="dwKeySize">
        /// Rozmiar klucza jako int domyślnie 4096
        /// The key size as int defaults to 4096
        /// </param>
        /// <returns>
        /// samego siebie jako RsaServiceProvider
        /// self as RsaServiceProvider
        /// </returns>
        public virtual RsaProviderService Initialize(int dwKeySize = DwKeySize)
        {
            try
            {
                if (null == RsaKeyPairGenerator)
                {
                    using (var rsa = new RSACryptoServiceProvider(dwKeySize))
                    {
                        RsaKeyPairGenerator = new RsaKeyPairGenerator();
                        RsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), dwKeySize));
                    }
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return this;
        }
        #endregion

        #region public virtual async Task<RsaServiceProvider> InitializeAsync(int dwKeySize = DwKeySize)
        /// <summary>
        /// Generuj asynchronicznie
        /// Initialize asynchronously
        /// <para>
        /// Inicjuje RsaKeyPairGenerator przez wywołanie new RsaKeyPairGenerator(); co umożliwia stosowanie instancji bez ręcznego ustawiania parametrów
        /// Initializes the RsaKeyPairGenerator by calling new RsaKeyPairGenerator (); which enables the use of instances without manual parameter setting
        /// </para>
        /// </summary>
        /// <param name="dwKeySize">
        /// Rozmiar klucza jako int domyślnie 4096
        /// The key size as int defaults to 4096
        /// </param>
        /// <returns>
        /// samego siebie jako RsaServiceProvider
        /// self as RsaServiceProvider
        /// </returns>
        public virtual async Task<RsaProviderService> InitializeAsync(int dwKeySize = DwKeySize)
        {
            return await Task.Run(() =>
            {
                return Initialize(dwKeySize);
            });
        }
        #endregion

        public virtual string GetAsymmetricPrivateKeyAsString(string asymmetricPrivateKeyFilePath = null)
        {
            try
            {
                asymmetricPrivateKeyFilePath ??= AsymmetricPrivateKeyFilePath;
                if (null != asymmetricPrivateKeyFilePath && File.Exists(asymmetricPrivateKeyFilePath))
                {
                    AsymmetricPrivateKeyAsString = File.ReadAllText(asymmetricPrivateKeyFilePath);
                }
                else
                {
                    using (TextWriter textWriter = new StringWriter())
                    {
                        var pemWriter = new PemWriter(textWriter);
                        pemWriter.WriteObject(AsymmetricPrivateKey);
                        pemWriter.Writer.Flush();
                        AsymmetricPrivateKeyAsString = textWriter.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return AsymmetricPrivateKeyAsString;
        }

        public virtual async Task<string> GetAsymmetricPrivateKeyAsStringAsync(string asymmetricPrivateKeyFilePath = null)
        {
            return await Task.Run(() =>
            {
                return GetAsymmetricPrivateKeyAsString(asymmetricPrivateKeyFilePath);
            });
        }

        public virtual string GetAsymmetricPublicKeyAsString(string asymmetricPublicKeyFilePath = null)
        {
            try
            {
                asymmetricPublicKeyFilePath ??= AsymmetricPublicKeyFilePath;
                if (null != asymmetricPublicKeyFilePath && File.Exists(asymmetricPublicKeyFilePath))
                {
                    AsymmetricPublicKeyAsString = File.ReadAllText(asymmetricPublicKeyFilePath);
                }
                else
                {
                    using (TextWriter textWriter = new StringWriter())
                    {
                        var pemWriter = new PemWriter(textWriter);
                        pemWriter.WriteObject(AsymmetricPublicKey);
                        pemWriter.Writer.Flush();
                        AsymmetricPublicKeyAsString = textWriter.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return AsymmetricPublicKeyAsString;
        }

        public virtual async Task<string> GetAsymmetricPublicKeyAsStringAsync(string asymmetricPublicKeyFilePath = null)
        {
            return await Task.Run(() =>
            {
                return GetAsymmetricPublicKeyAsString(asymmetricPublicKeyFilePath);
            });
        }

        private void SaveAsymmetricPrivateKeyToFile(string asymmetricPrivateKeyFilePath = null, bool checkIfExists = false)
        {
            try
            {
                asymmetricPrivateKeyFilePath ??= AsymmetricPrivateKeyFilePath;
                if (null != asymmetricPrivateKeyFilePath && (checkIfExists == false || (checkIfExists == true && !File.Exists(asymmetricPrivateKeyFilePath))))
                {
                    var directory = Path.GetDirectoryName(asymmetricPrivateKeyFilePath);
                    if (null != directory && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.WriteAllText(asymmetricPrivateKeyFilePath, AsymmetricPrivateKeyAsString);
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
        }

        private void SaveAsymmetricPublicKeyToFile(string asymmetricPublicKeyFilePath = null, bool checkIfExists = false)
        {
            try
            {
                asymmetricPublicKeyFilePath ??= AsymmetricPublicKeyFilePath;
                if (null != asymmetricPublicKeyFilePath && (checkIfExists == false || (checkIfExists == true && !File.Exists(asymmetricPublicKeyFilePath))))
                {
                    var directory = Path.GetDirectoryName(asymmetricPublicKeyFilePath);
                    if (null != directory && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.WriteAllText(asymmetricPublicKeyFilePath, AsymmetricPublicKeyAsString);
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
        }

        public virtual void SaveAsymmetricKeyPairToFile(string asymmetricPrivateKeyFilePath = null, string asymmetricPublicKeyFilePath = null, bool checkIfExists = false)
        {
            SaveAsymmetricPrivateKeyToFile(asymmetricPrivateKeyFilePath, checkIfExists);
            SaveAsymmetricPublicKeyToFile(asymmetricPublicKeyFilePath, checkIfExists);
        }

        public virtual string DecryptWithPublicKey(string text, string publicKey = null)
        {
            try
            {
                if (null != text && !string.IsNullOrWhiteSpace(text))
                {
                    publicKey ??= AsymmetricPublicKeyAsString;
                    //Console.WriteLine(publicKey);
                    var bytesToDecrypt = Convert.FromBase64String(text);
                    var stringReader = new StringReader(publicKey);
                    var pemReader = new PemReader(stringReader);
                    var @object = pemReader.ReadObject();
                    var asymmetricKeyParameter = (AsymmetricKeyParameter)@object;
                    //Pkcs1Encoding.StrictLengthEnabled = false;
                    var pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
                    pkcs1Encoding.Init(false, asymmetricKeyParameter);
                    return Encoding.UTF8.GetString(pkcs1Encoding.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return text;
        }

        public virtual string DecryptWithPrivateKey(string text, string privateKey = null)
        {
            try
            {
                if (null != text && !string.IsNullOrWhiteSpace(text))
                {
                    privateKey ??= AsymmetricPrivateKeyAsString;
                    //Console.WriteLine(privateKey);
                    var bytesToDecrypt = Convert.FromBase64String(text);
                    var stringReader = new StringReader(privateKey);
                    var pemReader = new PemReader(stringReader);
                    var @object = pemReader.ReadObject();
                    var asymmetricCipherKeyPair = (AsymmetricCipherKeyPair)@object;
                    //Pkcs1Encoding.StrictLengthEnabled = false;
                    var pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
                    pkcs1Encoding.Init(false, asymmetricCipherKeyPair.Public);
                    return Encoding.UTF8.GetString(pkcs1Encoding.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return text;
        }

        public virtual string EncryptWithPrivateKey(string text, string privateKey = null)
        {
            try
            {
                if (null != text && !string.IsNullOrWhiteSpace(text))
                {
                    privateKey ??= AsymmetricPrivateKeyAsString;
                    var bytesToEncrypt = Encoding.UTF8.GetBytes(text);
                    var stringReader = new StringReader(privateKey);
                    var pemReader = new PemReader(stringReader);
                    var @object = pemReader.ReadObject();
                    var asymmetricCipherKeyPair = (AsymmetricCipherKeyPair)@object;
                    //Pkcs1Encoding.StrictLengthEnabled = false;
                    var encryptEngine = new Pkcs1Encoding(new RsaEngine());
                    encryptEngine.Init(true, asymmetricCipherKeyPair.Private);
                    return Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
            return text;
        }
    }
}
