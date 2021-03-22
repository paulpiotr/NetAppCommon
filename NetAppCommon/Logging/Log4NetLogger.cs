#region using

using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;

#endregion

namespace NetAppCommon.Logging
{
    public class Log4NetLogger
    {
        /// <summary>
        /// </summary>
        static Log4NetLogger()
        {
            ILoggerRepository repository = Log4NetInstance.Logger.Repository;
            var fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\" + "log4net.config"));
            XmlConfigurator.ConfigureAndWatch(repository, fileInfo);
        }

        /// <summary>
        /// </summary>
        public static Type TypeofLogger { get; set; }

        /// <summary>
        /// </summary>
        public static ILog Log4NetInstance { get; } =
            LogManager.GetLogger(TypeofLogger ?? MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>
        ///     ILog
        /// </returns>
        public static ILog GetLog4NetInstance(Type t)
        {
            TypeofLogger = t;
            return LogManager.GetLogger(t);
        }
    }
}
