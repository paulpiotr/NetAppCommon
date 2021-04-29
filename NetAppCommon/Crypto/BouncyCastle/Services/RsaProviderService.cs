#region using

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NetAppCommon.Crypto.BouncyCastle.Models.Base;
using NetAppCommon.Crypto.BouncyCastle.Services.Interface;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

#endregion

namespace NetAppCommon.Crypto.BouncyCastle.Services
{
    public sealed class RsaProviderService : RsaProvaiderBaseModel, IRsaProviderService
    {
        #region private const int DwKeySize = 4096;

        /// <summary>
        ///     Rozmiar klucza - domyślna wartość jako int
        ///     Key size - default value as int
        /// </summary>
        private const int DwKeySize = 4096;

        #endregion

        #region private readonly log4net.ILog _log4Net

        /// <summary>
        ///     Referencja klasy Log4NetLogger
        ///     Reference to the Log4NetLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public RsaProviderService()

        /// <summary>
        ///     Konstruktor
        ///     Constructor
        /// </summary>
        public RsaProviderService()
        {
            Initialize();
        }

        #endregion

        #region public RsaProviderService(int dwKeySize = DwKeySize)

        /// <summary>
        ///     Konstruktor
        ///     Constructor
        /// </summary>
        /// <param name="dwKeySize">
        ///     Rozmiar klucza jako int
        ///     Key size as int
        /// </param>
        public RsaProviderService(int dwKeySize = DwKeySize)
        {
            Initialize(dwKeySize);
        }

        #endregion

        public RsaProviderService(string asymmetricPrivateKeyFilePath, string asymmetricPublicKeyFilePath,
            bool initialize = true, int dwKeySize = DwKeySize)
        {
            if (initialize)
            {
                Initialize(dwKeySize);
            }

            AsymmetricPrivateKeyFilePath = asymmetricPrivateKeyFilePath;
            AsymmetricPublicKeyFilePath = asymmetricPublicKeyFilePath;
        }

        #region public virtual RsaServiceProvider Initialize(int dwKeySize = DwKeySize)

        /// <summary>
        ///     Inicjuj
        ///     Initialize
        ///     <para>
        ///         Inicjuje RsaKeyPairGenerator przez wywołanie new RsaKeyPairGenerator(); co umożliwia stosowanie instancji bez
        ///         ręcznego ustawiania parametrów
        ///         Initializes the RsaKeyPairGenerator by calling new RsaKeyPairGenerator (); which enables the use of instances
        ///         without manual parameter setting
        ///     </para>
        /// </summary>
        /// <param name="dwKeySize">
        ///     Rozmiar klucza jako int domyślnie 4096
        ///     The key size as int defaults to 4096
        /// </param>
        /// <returns>
        ///     samego siebie jako RsaServiceProvider
        ///     self as RsaServiceProvider
        /// </returns>
        public RsaProviderService Initialize(int dwKeySize = DwKeySize)
        {
            try
            {
                lock (string.Intern(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName))
                {
                    using var rsa = new RSACryptoServiceProvider(dwKeySize);
                    RsaKeyPairGenerator = new RsaKeyPairGenerator();
                    RsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), dwKeySize));
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return this;
        }

        #endregion

        #region public virtual async Task<RsaServiceProvider> InitializeAsync(int dwKeySize = DwKeySize)

        /// <summary>
        ///     Generuj asynchronicznie
        ///     Initialize asynchronously
        ///     <para>
        ///         Inicjuje RsaKeyPairGenerator przez wywołanie new RsaKeyPairGenerator(); co umożliwia stosowanie instancji bez
        ///         ręcznego ustawiania parametrów
        ///         Initializes the RsaKeyPairGenerator by calling new RsaKeyPairGenerator (); which enables the use of instances
        ///         without manual parameter setting
        ///     </para>
        /// </summary>
        /// <param name="dwKeySize">
        ///     Rozmiar klucza jako int domyślnie 4096
        ///     The key size as int defaults to 4096
        /// </param>
        /// <returns>
        ///     samego siebie jako RsaServiceProvider
        ///     self as RsaServiceProvider
        /// </returns>
        public async Task<RsaProviderService> InitializeAsync(int dwKeySize = DwKeySize) =>
            await Task.Run(() => Initialize(dwKeySize));

        #endregion

        public string GetAsymmetricPrivateKeyAsString(string asymmetricPrivateKeyFilePath = null)
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
                    using TextWriter textWriter = new StringWriter();
                    var pemWriter = new PemWriter(textWriter);
                    pemWriter.WriteObject(AsymmetricPrivateKey);
                    pemWriter.Writer.Flush();
                    AsymmetricPrivateKeyAsString = textWriter.ToString();
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return AsymmetricPrivateKeyAsString;
        }

        public async Task<string> GetAsymmetricPrivateKeyAsStringAsync(
            string asymmetricPrivateKeyFilePath = null) =>
            await Task.Run(() => GetAsymmetricPrivateKeyAsString(asymmetricPrivateKeyFilePath));

        public string GetAsymmetricPublicKeyAsString(string asymmetricPublicKeyFilePath = null)
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
                    using TextWriter textWriter = new StringWriter();
                    var pemWriter = new PemWriter(textWriter);
                    pemWriter.WriteObject(AsymmetricPublicKey);
                    pemWriter.Writer.Flush();
                    AsymmetricPublicKeyAsString = textWriter.ToString();
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return AsymmetricPublicKeyAsString;
        }

        public async Task<string>
            GetAsymmetricPublicKeyAsStringAsync(string asymmetricPublicKeyFilePath = null) =>
            await Task.Run(() => GetAsymmetricPublicKeyAsString(asymmetricPublicKeyFilePath));

        #region public static RsaProviderService GetRsaProviderService()

        /// <summary>
        ///     Pobierz statyczną referencję klasy RsaProviderService
        ///     Get a static RsaProviderService class reference
        /// </summary>
        /// <returns>
        ///     Statyczna referencja jako RsaProviderService
        ///     Static reference as Rsa Provider Service
        /// </returns>
        public static RsaProviderService GetRsaProviderService() => new();

        #endregion

        public static RsaProviderService GetRsaProviderService(int dwKeySize) => new(dwKeySize);

        public static RsaProviderService GetRsaProviderService(string asymmetricPrivateKeyFilePath,
            string asymmetricPublicKeyFilePath, bool initialize = true, int dwKeySize = DwKeySize) =>
            new(asymmetricPrivateKeyFilePath, asymmetricPublicKeyFilePath, initialize, dwKeySize);

        private void SaveAsymmetricPrivateKeyToFile(string asymmetricPrivateKeyFilePath = null,
            bool checkIfExists = false)
        {
            try
            {
                asymmetricPrivateKeyFilePath ??= AsymmetricPrivateKeyFilePath;
                if (null != asymmetricPrivateKeyFilePath && (checkIfExists == false ||
                                                             !File.Exists(asymmetricPrivateKeyFilePath)))
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
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }
        }

        private void SaveAsymmetricPublicKeyToFile(string asymmetricPublicKeyFilePath = null,
            bool checkIfExists = false)
        {
            try
            {
                asymmetricPublicKeyFilePath ??= AsymmetricPublicKeyFilePath;
                if (null != asymmetricPublicKeyFilePath && (checkIfExists == false ||
                                                            !File.Exists(asymmetricPublicKeyFilePath)))
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
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }
        }

        public void SaveAsymmetricKeyPairToFile(string asymmetricPrivateKeyFilePath = null,
            string asymmetricPublicKeyFilePath = null, bool checkIfExists = false)
        {
            SaveAsymmetricPrivateKeyToFile(asymmetricPrivateKeyFilePath, checkIfExists);
            SaveAsymmetricPublicKeyToFile(asymmetricPublicKeyFilePath, checkIfExists);
        }

        public string DecryptWithPublicKey(string text, string publicKey = null)
        {
            try
            {
                if (null != text && !string.IsNullOrWhiteSpace(text))
                {
                    publicKey ??= AsymmetricPublicKeyAsString;
                    if (null != publicKey)
                    {
                        var bytesToDecrypt = Convert.FromBase64String(text);
                        var stringReader = new StringReader(publicKey);
                        var pemReader = new PemReader(stringReader);
                        object @object = pemReader.ReadObject();
                        if (@object is { } o)
                        {
                            var asymmetricKeyParameter = (AsymmetricKeyParameter)o;
                            //Pkcs1Encoding.StrictLengthEnabled = false;
                            var pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
                            pkcs1Encoding.Init(false, asymmetricKeyParameter);
                            return Encoding.UTF8.GetString(pkcs1Encoding.ProcessBlock(bytesToDecrypt, 0,
                                bytesToDecrypt.Length));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return text;
        }

        public string DecryptWithPrivateKey(string text, string privateKey = null)
        {
            try
            {
                if (null != text && !string.IsNullOrWhiteSpace(text))
                {
                    privateKey ??= AsymmetricPrivateKeyAsString;
                    if (null != privateKey)
                    {
                        var bytesToDecrypt = Convert.FromBase64String(text);
                        var stringReader = new StringReader(privateKey);
                        var pemReader = new PemReader(stringReader);
                        object @object = pemReader.ReadObject();
                        if (@object is { } o)
                        {
                            var asymmetricCipherKeyPair = (AsymmetricCipherKeyPair)o;
                            //Pkcs1Encoding.StrictLengthEnabled = false;
                            var pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
                            pkcs1Encoding.Init(false, asymmetricCipherKeyPair.Public);
                            return Encoding.UTF8.GetString(pkcs1Encoding.ProcessBlock(bytesToDecrypt, 0,
                                bytesToDecrypt.Length));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return text;
        }

        public string EncryptWithPrivateKey(string text, string privateKey = null)
        {
            try
            {
                if (null != text && !string.IsNullOrWhiteSpace(text))
                {
                    privateKey ??= AsymmetricPrivateKeyAsString;
                    if (null != privateKey)
                    {
                        var bytesToEncrypt = Encoding.UTF8.GetBytes(text);
                        var stringReader = new StringReader(privateKey);
                        var pemReader = new PemReader(stringReader);
                        object @object = pemReader.ReadObject();
                        var asymmetricCipherKeyPair = (AsymmetricCipherKeyPair)@object;
                        //Pkcs1Encoding.StrictLengthEnabled = false;
                        var encryptEngine = new Pkcs1Encoding(new RsaEngine());
                        encryptEngine.Init(true, asymmetricCipherKeyPair.Private);
                        return Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0,
                            bytesToEncrypt.Length));
                    }
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return text;
        }
    }
}
