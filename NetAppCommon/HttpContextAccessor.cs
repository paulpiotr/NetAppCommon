using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetAppCommon
{
    public class HttpContextAccessor
    {
        public static class AppContext
        {

            #region private static readonly log4net.ILog _log4net
            /// <summary>
            /// Log4 Net Logger
            /// </summary>
            private static readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
            #endregion

            private static IHttpContextAccessor _httpContextAccessor;

            public static void Configure(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }

            public static HttpContext HttpContextCurrent => _httpContextAccessor?.HttpContext ?? null;

            public static string GetCurrentUserIdentityName()
            {
                try
                {
                    if (null != HttpContextCurrent)
                    {
                        return HttpContextCurrent.User.Identity.Name;
                    }
                    else
                    {
                        return Environment.UserName;
                    }
                }
                catch (Exception e)
                {
                    _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                }
                return null;
            }
        }
    }
}
