#region using

using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

#endregion

namespace NetAppCommon.Helpers.Compression
{
    public class ZipHelper
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Log4net Logger
        ///     Log4net Logger
        /// </summary>
        private static readonly ILog Log4Net =
            Logging.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        public static async Task<byte[]> ZipAsync(string @string)
        {
            return await Task.Run(() => Zip(@string));
        }

        public static byte[] Zip(string @string)
        {
            byte[] compressed = null;
            try
            {
                using var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                {
                    using (var streamWriter = new StreamWriter(gZipStream, Encoding.UTF8))
                    {
                        streamWriter.Write(@string);
                    }
                }

                compressed = memoryStream.ToArray();
            }
            catch (Exception e)
            {
                Log4Net.Error(e);
                if (null != e.InnerException)
                {
                    Log4Net.Error(e.InnerException);
                }
            }

            return compressed ?? Encoding.UTF8.GetBytes(@string);
        }

        public static string UnZip(byte[] compressed)
        {
            string @string = null;
            try
            {
                using var memoryStream = new MemoryStream(compressed);
                using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                using var streamReader = new StreamReader(gZipStream, Encoding.UTF8);
                @string = streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Log4Net.Error(e);
                if (null != e.InnerException)
                {
                    Log4Net.Error(e.InnerException);
                }
            }

            return @string;
        }

        public static async Task<string> UnZipAsync(byte[] compressed)
        {
            return await Task.Run(() => UnZip(compressed));
        }
    }
}