using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using NetAppCommon.Validation;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;

namespace NetAppCommon.Crypto.BouncyCastle.Models.Base
{
    public class RsaProvaiderBaseModel
    {
        #region private readonly log4net.ILog _log4net
        /// <summary>
        /// Referencja klasy Log4NetLogget
        /// Reference to the Log4NetLogget class
        /// </summary>
        private readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region protected RsaKeyPairGenerator _rsaKeyPairGenerator; public RsaKeyPairGenerator RsaKeyPairGenerator

        protected RsaKeyPairGenerator _rsaKeyPairGenerator;

        [JsonIgnore]
        [XmlIgnore]
        [NotRequired]
        public RsaKeyPairGenerator RsaKeyPairGenerator
        {
            get => _rsaKeyPairGenerator;
            set
            {
                if (value != _rsaKeyPairGenerator)
                {
                    _rsaKeyPairGenerator = value;
                }
            }
        }

        #endregion

        #region protected AsymmetricCipherKeyPair _keyPair; public AsymmetricCipherKeyPair KeyPair

        protected AsymmetricCipherKeyPair _keyPair;

        [JsonIgnore]
        [XmlIgnore]
        [NotRequired]
        public AsymmetricCipherKeyPair KeyPair
        {
            get
            {
                if (null != RsaKeyPairGenerator && null == _keyPair)
                {
                    _keyPair = RsaKeyPairGenerator.GenerateKeyPair();
                }
                return _keyPair;
            }
            set
            {
                if (value != _keyPair)
                {
                    _keyPair = value;
                }
            }
        }
        #endregion

        #region protected AsymmetricKeyParameter _asymmetricPrivateKey; public AsymmetricKeyParameter AsymmetricPrivateKey

        protected AsymmetricKeyParameter _asymmetricPrivateKey;

        [JsonIgnore]
        [XmlIgnore]
        [NotRequired]
        public AsymmetricKeyParameter AsymmetricPrivateKey
        {
            get
            {
                if (null != KeyPair && null == _asymmetricPrivateKey)
                {
                    _asymmetricPrivateKey = KeyPair.Private;
                }
                return _asymmetricPrivateKey;
            }
            set
            {
                if (value != _asymmetricPrivateKey)
                {
                    _asymmetricPrivateKey = value;
                }
            }
        }
        #endregion

        #region protected AsymmetricKeyParameter _asymmetricPublicKey; public AsymmetricKeyParameter AsymmetricPublicKey

        protected AsymmetricKeyParameter _asymmetricPublicKey;

        [JsonIgnore]
        [XmlIgnore]
        [NotRequired]
        public AsymmetricKeyParameter AsymmetricPublicKey
        {
            get
            {
                if (null != KeyPair && null == _asymmetricPublicKey)
                {
                    _asymmetricPublicKey = KeyPair.Public;
                }
                return _asymmetricPublicKey;
            }
            set
            {
                if (value != _asymmetricPublicKey)
                {
                    _asymmetricPublicKey = value;
                }
            }
        }
        #endregion

        #region protected string _asymmetricPrivateKeyAsString; public string AsymmetricPrivateKeyAsString

        protected string _asymmetricPrivateKeyAsString;

        [JsonIgnore]
        [XmlIgnore]
        [NotRequired]
        public string AsymmetricPrivateKeyAsString
        {
            get
            {
                if (null == _asymmetricPrivateKeyAsString)
                {
                    try
                    {
                        if (null != AsymmetricPrivateKeyFilePath && File.Exists(AsymmetricPrivateKeyFilePath))
                        {
                            _asymmetricPrivateKeyAsString = File.ReadAllText(AsymmetricPrivateKeyFilePath);
                        }
                        else if (null != AsymmetricPrivateKey)
                        {
                            using (TextWriter textWriter = new StringWriter())
                            {
                                var pemWriter = new PemWriter(textWriter);
                                pemWriter.WriteObject(AsymmetricPrivateKey);
                                pemWriter.Writer.Flush();
                                _asymmetricPrivateKeyAsString = textWriter.ToString();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
                    }
                }
                return _asymmetricPrivateKeyAsString;
            }
            set
            {
                if (value != _asymmetricPrivateKeyAsString)
                {
                    _asymmetricPrivateKeyAsString = value;
                }
            }
        }
        #endregion

        #region protected string _asymmetricPublicKeyAsString; public string AsymmetricPublicKeyAsString

        protected string _asymmetricPublicKeyAsString;

        [JsonIgnore]
        [XmlIgnore]
        [NotRequired]
        public string AsymmetricPublicKeyAsString
        {
            get
            {
                if (null == _asymmetricPublicKeyAsString)
                {
                    if (null != AsymmetricPublicKeyFilePath && File.Exists(AsymmetricPublicKeyFilePath))
                    {
                        _asymmetricPublicKeyAsString = File.ReadAllText(AsymmetricPublicKeyFilePath);
                        //Console.WriteLine($"\n\n\n { _asymmetricPublicKeyAsString } \n\n\n");
                    }
                    else if (null != AsymmetricPublicKey)
                    {
                        using (TextWriter textWriter = new StringWriter())
                        {
                            var pemWriter = new PemWriter(textWriter);
                            pemWriter.WriteObject(AsymmetricPublicKey);
                            pemWriter.Writer.Flush();
                            _asymmetricPublicKeyAsString = textWriter.ToString();
                            //Console.WriteLine($"\n\n\n { _asymmetricPublicKeyAsString } \n\n\n");
                        }
                    }
                }
                return _asymmetricPublicKeyAsString;
            }
            set
            {
                if (value != _asymmetricPublicKeyAsString)
                {
                    _asymmetricPublicKeyAsString = value;
                }
            }
        }
        #endregion

        protected string _asymmetricPrivateKeyFilePath;

        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public string AsymmetricPrivateKeyFilePath
        {
            get => _asymmetricPrivateKeyFilePath;
            set
            {
                if (value != _asymmetricPrivateKeyFilePath)
                {
                    _asymmetricPrivateKeyFilePath = value;
                }
            }
        }

        protected string _asymmetricPublicKeyFilePath;

        [XmlIgnore]
        [JsonIgnore]
        [NotRequired]
        public string AsymmetricPublicKeyFilePath
        {
            get => _asymmetricPublicKeyFilePath;
            set
            {
                if (value != _asymmetricPublicKeyFilePath)
                {
                    _asymmetricPublicKeyFilePath = value;
                }
            }
        }
    }
}
