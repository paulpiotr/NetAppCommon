#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

#endregion

namespace NetAppCommon.Helpers.Lists;

public class ListsHelper
{
    #region private readonly log4net.ILog log4net

    /// <summary>
    ///     Log4net Logger
    ///     Log4net Logger
    /// </summary>
    private readonly ILog _log4Net =
        Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

    #endregion

    public char[] DelimiterChars { get; set; } = { ',', ';' };

    public static ListsHelper GetInstance() => new();

    public List<string> ConvertToListOfString(string delimiterSeparatedAttributes, char[] delimiterChars = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(delimiterSeparatedAttributes))
            {
                delimiterChars ??= DelimiterChars;
                return new List<string>(delimiterSeparatedAttributes.Split(delimiterChars)).Select(x => x.Trim())
                    .ToList();
            }
        }
        catch (Exception e)
        {
            _log4Net.Error(e.Message, e);
            if (null != e.InnerException)
            {
                _log4Net.Error(e.InnerException.Message, e.InnerException);
            }
        }

        return null;
    }

    public async Task<List<string>> ConvertToListOfStringAsync(string delimiterSeparatedAttributes,
        char[] delimiterChars = null) =>
        await Task.Run(() => ConvertToListOfString(delimiterSeparatedAttributes, delimiterChars));
}
