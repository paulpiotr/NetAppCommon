using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetAppCommon.Helpers.Lists
{
    public class ListsHelper
    {
        #region private static readonly log4net.ILog log4net
        /// <summary>
        /// Log4net Logger
        /// Log4net Logger
        /// </summary>
        private static readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public char[] DelimiterChars { get; set; } = { ',', ';' };

        public static ListsHelper GetInstance()
        {
            return new ListsHelper();
        }
        
        public List<string> ConvertToListOfString (string delimiterSeparatedAttributes, char[] delimiterChars = null)
        {
            try
            {
                delimiterChars = delimiterChars ?? DelimiterChars;
                return new List<string>(delimiterSeparatedAttributes.Split(delimiterChars)).Select(x => x.Trim()).ToList();
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
            }
            return null;
        }

        public async Task<List<string>> ConvertToListOfStringAsync(string delimiterSeparatedAttributes, char[] delimiterChars = null)
        {
            return await Task.Run(() => {
                return ConvertToListOfString(delimiterSeparatedAttributes, delimiterChars);
            });
        }
    }
}
