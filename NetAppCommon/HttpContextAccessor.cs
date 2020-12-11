using System;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace NetAppCommon
{
    public class HttpContextAccessor
    {
        public static class AppContext
        {
            private static readonly log4net.ILog Log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);

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
                        return HttpContextCurrent.User.Identity.Name ?? string.Empty;
                    }
                    else
                    {
                        return Environment.UserName ?? string.Empty;
                    }
                }
                catch (Exception e)
                {
                    Log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
                }
                return string.Empty;
            }
        }
    }
}
