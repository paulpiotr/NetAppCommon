#region using

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using NetAppCommon.Crypto.AesCryptography.Services.Interface;

#endregion

namespace NetAppCommon.Crypto.AesCryptography.Services
{
    public class AesIVProviderService : IAesIVProviderService
    {
        #region private readonly log4net.ILog _log4Net

        /// <summary>
        ///     Referencja klasy Log4NetLogger
        ///     Reference to the Log4NetLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <param name="salt">
        /// </param>
        /// <returns>
        /// </returns>
        public string Decpypt(string text, string salt)
        {
            try
            {
                salt = !string.IsNullOrWhiteSpace(salt) ? salt : Marshal.GenerateGuidForType(GetType()).ToString();
                var src = Convert.FromBase64String(text);
                var dst = new byte[16];
                var cipher = new byte[src.Length - dst.Length];
                Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
                Buffer.BlockCopy(src, dst.Length, cipher, 0, cipher.Length);
                var key = new MD5CryptoServiceProvider().ComputeHash(
                    Encoding.UTF8.GetBytes(ClearTextFromWhitespace(salt)));
                using (var aes = Aes.Create())
                {
                    using (ICryptoTransform cryptoTransform = aes.CreateDecryptor(key, dst))
                    {
                        using (var memoryStream = new MemoryStream(cipher))
                        {
                            using (var cryptoStream =
                                new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                            {
                                using (var streamReader = new StreamReader(cryptoStream))
                                {
                                    return streamReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }

            return text;
        }

        /// <summary>
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <param name="salt">
        /// </param>
        /// <returns>
        /// </returns>
        public async Task<string> DecpyptAsync(string text, string salt) =>
            await Task.Run(() =>
            {
                return Decpypt(text, salt);
            });

        #region public string Encrypt(string text, string salt)

        /// <summary>
        ///     Zaszyfruj
        ///     Encrypt
        /// </summary>
        /// <param name="text">
        ///     Tekst do zaszyfrowania jako string
        ///     Text to encrypt as string
        /// </param>
        /// <param name="salt">
        ///     Sól jako string
        ///     Salt as string
        /// </param>
        /// <returns>
        ///     Zaszyfrowany tekst lub oryginalny tekst jako string
        ///     The encrypted text or the original text as a string
        /// </returns>
        public string Encrypt(string text, string salt)
        {
            try
            {
                salt = !string.IsNullOrWhiteSpace(salt) ? salt : Marshal.GenerateGuidForType(GetType()).ToString();
                var rgbKey =
                    new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(ClearTextFromWhitespace(salt)));
                using (var aes = Aes.Create())
                {
                    using (ICryptoTransform cryptoTransform = aes.CreateEncryptor(rgbKey, aes.IV))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream =
                                new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                            {
                                using (var streamWriter = new StreamWriter(cryptoStream))
                                {
                                    streamWriter.Write(text);
                                }

                                var src = aes.IV;
                                var dst = memoryStream.ToArray();
                                var result = new byte[src.Length + dst.Length];
                                Buffer.BlockCopy(src, 0, result, 0, src.Length);
                                Buffer.BlockCopy(dst, 0, result, src.Length, dst.Length);
                                text = Convert.ToBase64String(result);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }

            return text;
        }

        #endregion

        #region public async Task<string> EncryptAsync(string text, string salt)

        public async Task<string> EncryptAsync(string text, string salt) =>
            await Task.Run(() =>
            {
                return Encrypt(text, salt);
            });

        #endregion

        #region private string ClearTextFromWhitespace(string text)

        /// <summary>
        ///     Wyczyść tekst z białych znaków
        ///     Clear text from whitespace
        /// </summary>
        /// <param name="text">
        ///     tekst jako string
        ///     text as string
        /// </param>
        /// <returns>
        ///     Sformatowany lub originaly tekst jako string
        ///     Formatted or original text as string
        /// </returns>
        private string ClearTextFromWhitespace(string text)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    text = Regex.Replace(text, @"\s+|\r+|\n+|\r\n+", string.Empty);
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }

            return text;
        }

        #endregion
    }
}
