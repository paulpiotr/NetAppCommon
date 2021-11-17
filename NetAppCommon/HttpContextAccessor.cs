#region using

using System;
using System.Reflection;
using log4net;
using Microsoft.AspNetCore.Http;

#endregion

namespace NetAppCommon;

public class HttpContextAccessor
{
    public static class AppContext
    {
        private static readonly ILog Log4net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        private static IHttpContextAccessor _httpContextAccessor;

        public static HttpContext HttpContextCurrent => _httpContextAccessor?.HttpContext ?? null;

        public static void Configure(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor;

        public static string GetCurrentUserIdentityName()
        {
            try
            {
                if (null != HttpContextCurrent)
                {
                    return HttpContextCurrent.User.Identity.Name ?? string.Empty;
                }

                return Environment.UserName ?? string.Empty;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }

            return string.Empty;
        }
    }
}
