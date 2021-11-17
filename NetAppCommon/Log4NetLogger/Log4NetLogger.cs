#region using

using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;

#endregion

namespace NetAppCommon.Log4NetLogger;

public class Log4NetLogger
{
    /// <summary>
    ///     static Log4netLogger()
    /// </summary>
    static Log4NetLogger()
    {
        ILoggerRepository repository = Log4NetInstance.Logger.Repository;
        var fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\" + "log4net.config"));
        XmlConfigurator.ConfigureAndWatch(repository, fileInfo);
    }

    /// <summary>
    ///     public static System.Type TypeofLogger { get; set; }
    /// </summary>
    public static Type TypeofLogger { get; set; }

    /// <summary>
    ///     public static ILog Log4netInstance { get; } = LogManager.GetLogger(TypeofLogger ??
    ///     System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
    /// </summary>
    public static ILog Log4NetInstance { get; } =
        LogManager.GetLogger(TypeofLogger ?? MethodBase.GetCurrentMethod()?.DeclaringType);

    /// <summary>
    ///     public static ILog GetLog4netInstance(System.Type t)
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
