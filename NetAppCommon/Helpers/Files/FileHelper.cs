#region using

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

#endregion

namespace NetAppCommon.Helpers.Files
{
    #region public class FileHelper

    /// <summary>
    ///     Pomocnik plików
    ///     File helper
    /// </summary>
    public class FileHelper
    {
        #region const int ErrorSharingViolation = 32;

        /// <summary>
        ///     Numer błędu (naruszenia) udostępniania
        ///     The sharing error (violation) number
        /// </summary>
        private const int ErrorSharingViolation = 32;

        #endregion

        #region const int ErrorLockViolation = 33;

        /// <summary>
        ///     Numer błędu (naruszenia) blokady
        ///     Lockout error (violation) number
        /// </summary>
        private const int ErrorLockViolation = 33;

        #endregion

        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Log4net Logger
        ///     Log4net Logger
        /// </summary>
        private static readonly ILog Log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public static FileHelper GetInstance()

        /// <summary>
        ///     Pobierz instancję do klasy NetAppCommon.Helpers.Files.FileHelper
        ///     Pobierz instancję do klasy NetAppCommon.Helpers.Files.FileHelper
        /// </summary>
        /// <returns>
        ///     Statuczny obiekt instancji klasy NetAppCommon.Helpers.Files.FileHelper
        ///     Statuczny obiekt instancji klasy NetAppCommon.Helpers.Files.FileHelper
        /// </returns>
        public static FileHelper GetInstance() => new();

        #endregion

        #region public static string GetMD5Hash(string filePath)

        /// <summary>
        ///     Pobierz skrót MD5 z treści pliku
        ///     Get the MD5 hash from the file content
        /// </summary>
        /// <param name="filePath">
        ///     Ścieżka do pliku jako string
        ///     File path as string
        /// </param>
        /// <returns>
        ///     Skrót MD5 z treści pliku jako string lub null
        ///     MD5 hash from the file content as string or null
        /// </returns>
        public static string GetMd5Hash(string filePath)
        {
            try
            {
                return Convert.ToBase64String(MD5.Create()
                    .ComputeHash(Encoding.UTF8.GetBytes(File.ReadAllText(filePath))));
            }
            catch (Exception e)
            {
#if DEBUG
                Log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
#else
                Console.WriteLine(e);
#endif
            }

            return null;
        }

        #endregion

        #region public async static Task<string> GetMD5HashAsync(string filePath)

        /// <summary>
        ///     Pobierz skrót MD5 z treści pliku asynchronicznie
        ///     Get the MD5 hash from the file content asynchronously
        /// </summary>
        /// <param name="filePath">
        ///     Ścieżka do pliku jako string
        ///     File path as string
        /// </param>
        /// <returns>
        ///     Skrót MD5 z treści pliku jako string lub null
        ///     MD5 hash from the file content as string or null
        /// </returns>
        public static async Task<string> GetMd5HashAsync(string filePath) => await Task.Run(() => GetMd5Hash(filePath));

        #endregion

        #region public static string GetMD5Hash(byte[] fileContent)

        /// <summary>
        ///     Pobierz skrót MD5 z treści pliku
        ///     Get the MD5 hash from the file content
        /// </summary>
        /// <param name="fileContent">
        ///     Treść pliku jako byte[]
        ///     File content as byte []
        /// </param>
        /// <returns>
        ///     Skrót MD5 z treści pliku jako string lub null
        ///     MD5 hash from the file content as string or null
        /// </returns>
        public static string GetMd5Hash(byte[] fileContent)
        {
            try
            {
                return Convert.ToBase64String(MD5.Create()
                    .ComputeHash(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(fileContent))));
            }
            catch (Exception e)
            {
#if DEBUG
                Log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
#else
                Console.WriteLine(e);
#endif
            }

            return null;
        }

        #endregion

        #region public async static Task<string> GetMD5HashAsync(byte[] fileContent)

        /// <summary>
        ///     Pobierz skrót MD5 z treści pliku asynchronicznie
        ///     Get the MD5 hash from the file content asynchronously
        /// </summary>
        /// <param name="fileContent">
        ///     Treść pliku jako byte[]
        ///     File content as byte []
        /// </param>
        /// <returns>
        ///     Skrót MD5 z treści pliku jako string lub null
        ///     MD5 hash from the file content as string or null
        /// </returns>
        public static async Task<string> GetMd5HashAsync(byte[] fileContent) =>
            await Task.Run(() => GetMd5Hash(fileContent));

        #endregion

        #region private bool IsLocked(Exception exception)

        /// <summary>
        ///     Sprawdź status błędu i zwróć prawdę, jeśli błąd dotyczy udostępniania lub blokady pliku lub fałsz jeśli inny
        ///     Check the error status and return True if the error is related to sharing or locking the file, or False if
        ///     different
        /// </summary>
        /// <param name="exception">
        ///     Wyjątek jako Exception
        ///     An exception as Exception
        /// </param>
        /// <returns>
        ///     prawda, jeśli błąd dotyczy udostępniania lub blokady pliku lub fałsz jeśli inny jako bool
        ///     true if the error is related to sharing or locking the file, or false if different as bool
        /// </returns>
        private bool IsLocked(Exception exception)
        {
            try
            {
                var errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
                return errorCode == ErrorSharingViolation || errorCode == ErrorLockViolation;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region private async Task<bool> IsLockedAsync(Exception exception)

        /// <summary>
        ///     Sprawdź status błędu i zwróć prawdę, jeśli błąd dotyczy udostępniania lub blokady pliku lub fałsz jeśli inny
        ///     asynchronicznie
        ///     Check the error status and return True if the error is related to sharing or locking the file, or False if
        ///     different asynchronously
        /// </summary>
        /// <param name="exception">
        ///     Wyjątek jako Exception
        ///     An exception as Exception
        /// </param>
        /// <returns>
        ///     prawda, jeśli błąd dotyczy udostępniania lub blokady pliku lub fałsz jeśli inny jako bool
        ///     true if the error is related to sharing or locking the file, or false if different as bool
        /// </returns>
        private async Task<bool> IsLockedAsync(Exception exception) => await Task.Run(() => IsLocked(exception));

        #endregion

        #region public void TimeoutAction<T>(Func<T> func, string filePath)

        /// <summary>
        ///     Dopuki plik jest zablokowany czekaj i wykonaj akcję po odblokowaniu pliku
        /// </summary>
        /// <typeparam name="T">
        ///     Parametr typu
        /// </typeparam>
        /// <param name="func">
        ///     Akcja jako func
        /// </param>
        /// <param name="filePath">
        ///     Ścieąka do pliku jako string
        /// </param>
        public void TimeoutAction<T>(Func<T> func, string filePath)
        {
            while (true)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        using FileStream fileStream =
                            File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                        fileStream.Close();
                        func();
                        break;
                    }
                }
                catch (Exception e) when (e is IOException)
                {
                    if (IsLocked(e))
                    {
                        Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                    }
                    else
                    {
#if DEBUG
                        Log4Net.Error(
                            $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
#else
                        Console.WriteLine($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n");
#endif
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Log4Net.Error(
                        $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
#else
                    Console.WriteLine($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n");
#endif
                }
            }
        }

        #endregion

        #region public void TimeoutAction<T>(Func<T> func, string filePath)

        /// <summary>
        ///     Dopuki plik jest zablokowany czekaj i wykonaj akcję po odblokowaniu pliku
        /// </summary>
        /// <typeparam name="T">
        ///     Parametr typu
        /// </typeparam>
        /// <param name="func">
        ///     Akcja jako func
        /// </param>
        /// <param name="filePath">
        ///     Ścieąka do pliku jako string
        /// </param>
        public T TimeoutActionReturn<T>(Func<T> func, string filePath)
        {
            while (true)
            {
                try
                {
                    using FileStream fileStream =
                        File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    fileStream.Close();
                    return func();
                }
                catch (Exception e) when (e is IOException)
                {
                    if (IsLocked(e))
                    {
                        Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                    }
                    else
                    {
#if DEBUG
                        Log4Net.Error(
                            $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
#else
                        Console.WriteLine($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n");
#endif
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Log4Net.Error(
                        $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
#else
                    Console.WriteLine($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n");
#endif
                }
            }
        }

        #endregion
    }

    #endregion
}
