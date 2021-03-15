#region using

using System;
using System.Reflection;
using System.Threading;
using log4net;

#endregion

namespace NetAppCommon.Helpers.Mutex
{
    public class MutexHelper
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Log4net Logger
        ///     Log4net Logger
        /// </summary>
        private readonly ILog _log4net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        private System.Threading.Mutex _mutex;

        public System.Threading.Mutex Mutex
        {
            get => _mutex;
            private set
            {
                if (value != _mutex)
                {
                    _mutex = value;
                }
            }
        }

        public MutexHelper MutexWaitOneAction<T>(Func<T> func, string name)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    lock (string.Intern(name))
                    {
                        if (!System.Threading.Mutex.TryOpenExisting(name, out var _mutex) && null == _mutex)
                        {
                            Console.WriteLine($"Run Mutex {name}");
                            _mutex ??= System.Threading.Mutex.OpenExisting(name) ??
                                       new System.Threading.Mutex(false, name);
                            func();
                            //_mutex.WaitOne();
                            Thread.Sleep((int) TimeSpan.FromSeconds(1).TotalMilliseconds * 1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                _log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
#else
                        Console.WriteLine($"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n");
#endif
            }

            return this;
        }
    }
}