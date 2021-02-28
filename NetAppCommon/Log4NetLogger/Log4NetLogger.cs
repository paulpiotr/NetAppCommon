using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;
using log4net.Repository;

namespace NetAppCommon.Log4NetLogger
{
    public class Log4NetLogger
    {
        /// <summary>
        /// public static System.Type TypeofLogger { get; set; }
        /// </summary>
        public static System.Type TypeofLogger { get; set; }
        /// <summary>
        /// public static ILog Log4netInstance { get; } = LogManager.GetLogger(TypeofLogger ?? System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        /// </summary>
        public static ILog Log4NetInstance { get; } = LogManager.GetLogger(TypeofLogger ?? System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        /// <summary>
        /// public static ILog GetLog4netInstance(System.Type t)
        /// </summary>
        /// <param name="t"></param>
        /// <returns>
        /// ILog
        /// </returns>
        public static ILog GetLog4NetInstance(Type t)
        {
            TypeofLogger = t;
            return LogManager.GetLogger(t);
        }
        /// <summary>
        /// static Log4netLogger()
        /// </summary>
        static Log4NetLogger()
        {
            ILoggerRepository repository = Log4NetInstance.Logger.Repository;
            var fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\" + "log4net.config"));
            log4net.Config.XmlConfigurator.ConfigureAndWatch(repository, fileInfo);
        }
    }
}
