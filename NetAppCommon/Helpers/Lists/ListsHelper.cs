#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

#endregion

namespace NetAppCommon.Helpers.Lists
{
    public class ListsHelper
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Log4net Logger
        ///     Log4net Logger
        /// </summary>
        private readonly ILog log4net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        public char[] DelimiterChars { get; set; } = {',', ';'};

        public static ListsHelper GetInstance()
        {
            return new();
        }

        public List<string> ConvertToListOfString(string delimiterSeparatedAttributes, char[] delimiterChars = null)
        {
            try
            {
                delimiterChars = delimiterChars ?? DelimiterChars;
                return new List<string>(delimiterSeparatedAttributes.Split(delimiterChars)).Select(x => x.Trim())
                    .ToList();
            }
            catch (Exception e)
            {
                log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }

            return null;
        }

        public async Task<List<string>> ConvertToListOfStringAsync(string delimiterSeparatedAttributes,
            char[] delimiterChars = null)
        {
            return await Task.Run(() =>
            {
                return ConvertToListOfString(delimiterSeparatedAttributes, delimiterChars);
            });
        }
    }
}