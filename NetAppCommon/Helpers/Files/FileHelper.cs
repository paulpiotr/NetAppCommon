using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetAppCommon.Helpers.Files
{
    #region public class FileHelper
    /// <summary>
    /// Pomocnik plików
    /// File helper
    /// </summary>
    public class FileHelper
    {
        #region private static readonly log4net.ILog log4net
        /// <summary>
        /// Log4net Logger
        /// Log4net Logger
        /// </summary>
        private static readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public static FileHelper GetInstance()
        {
            return new FileHelper();
        }

        #region public static string GetMD5Hash(string filePath)
        /// <summary>
        /// Pobierz skrót MD5 z treści pliku
        /// Get the MD5 hash from the file content
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        /// <returns>
        /// Skrót MD5 z treści pliku jako string lub null
        /// MD5 hash from the file content as string or null
        /// </returns>
        public static string GetMD5Hash(string filePath)
        {
            try
            {
                return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(System.IO.File.ReadAllText(filePath))));
            }
            catch (Exception e)
            {
#if DEBUG
                log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
#endif
            }
            return null;
        }
        #endregion

        #region public async static Task<string> GetMD5HashAsync(string filePath)
        /// <summary>
        /// Pobierz skrót MD5 z treści pliku asynchronicznie
        /// Get the MD5 hash from the file content asynchronously
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        /// <returns>
        /// Skrót MD5 z treści pliku jako string lub null
        /// MD5 hash from the file content as string or null
        /// </returns>
        public async static Task<string> GetMD5HashAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                return GetMD5Hash(filePath);
            });
        }
        #endregion

        #region public static string GetMD5Hash(byte[] fileContent)
        /// <summary>
        /// Pobierz skrót MD5 z treści pliku
        /// Get the MD5 hash from the file content
        /// </summary>
        /// <param name="fileContent">
        /// Treść pliku jako byte[]
        /// File content as byte []
        /// </param>
        /// <returns>
        /// Skrót MD5 z treści pliku jako string lub null
        /// MD5 hash from the file content as string or null
        /// </returns>
        public static string GetMD5Hash(byte[] fileContent)
        {
            try
            {
                return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(fileContent))));
            }
            catch (Exception e)
            {
#if DEBUG
                log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
#endif
            }
            return null;
        }
        #endregion

        #region public async static Task<string> GetMD5HashAsync(byte[] fileContent)
        /// <summary>
        /// Pobierz skrót MD5 z treści pliku asynchronicznie
        /// Get the MD5 hash from the file content asynchronously
        /// </summary>
        /// <param name="fileContent">
        /// Treść pliku jako byte[]
        /// File content as byte []
        /// </param>
        /// <returns>
        /// Skrót MD5 z treści pliku jako string lub null
        /// MD5 hash from the file content as string or null
        /// </returns>
        public async static Task<string> GetMD5HashAsync(byte[] fileContent)
        {
            return await Task.Run(() =>
            {
                return GetMD5Hash(fileContent);
            });
        }
        #endregion

        #region public static bool IsFileLocked(string filePath)
        /// <summary>
        /// Sprawdź, czy plik jest zablokowany przez inny proces
        /// Check if the file is locked by another process
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        /// <returns>
        /// True, jeśli plik jest zablokowany w przeciwnym razie false
        /// True if the file is locked otherwise false
        /// </returns>
        public static bool IsFileLocked(string filePath)
        {
            FileStream fileStream = null;
            try
            {
                /// NOTE: This doesn't handle situations where file is opened for writing by another process but put into write shared mode,
                /// it will not throw an exception and won't show it as write locked
                /// If we can't open file for reading and writing then it's locked by another process for writing
                fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (UnauthorizedAccessException)
            {
                try
                {
                    /// https://msdn.microsoft.com/en-us/library/y973b725(v=vs.110).aspx
                    /// This is because the file is Read-Only and we tried to open in ReadWrite mode, now try to open in Read only mode
                    fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (Exception)
                {
                    /// This file has been locked, we can't even open it to read
                    return true;
                }
            }
            catch (Exception)
            {
                /// This file has been locked
                return true;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return false;
        }
        #endregion

        #region public async static Task<bool> IsFileLockedAsync(string filePath)
        /// <summary>
        /// Sprawdź, czy plik jest zablokowany przez inny proces asynchronicznie
        /// Check if the file is locked by another process asynchronously
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        /// <returns>
        /// True, jeśli plik jest zablokowany w przeciwnym razie false
        /// True if the file is locked otherwise false
        /// </returns>
        public async static Task<bool> IsFileLockedAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                return IsFileLocked(filePath);
            });
        }
        #endregion

        public T TimeoutFileAction<T>(Func<T> func, int milliseconds = 100)
        {
            try
            {
                DateTime started = DateTime.UtcNow;
                while ((DateTime.UtcNow - started).TotalMilliseconds < milliseconds)
                {
                    try
                    {
#if DEBUG
                        log4net.Debug($"Check");
#endif
                        return func();
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
#endif
                    }
                }
                return default(T);
            }
            catch(Exception e)
            {
#if DEBUG
                log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
#endif
            }
            return default(T);
        }
    }
    #endregion
}
