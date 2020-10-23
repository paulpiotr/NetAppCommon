using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NetAppCommon.Helpers
{
    #region public class EntityContextHelper
    /// <summary>
    /// Entity Context Helper
    /// Pomocnik kontekstu jednostki
    /// </summary>
    public class EntityContextHelper
    {
        #region private static readonly log4net.ILog log4net
        /// <summary>
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public static async Task RunMigrationAsync<T>(IServiceProvider serviceProvider) where T : DbContext
        /// <summary>
        /// Uruchom migrację bazy danych asynchronicznie
        /// Run database migration asynchronously
        /// </summary>
        /// <param name="serviceProvider">
        /// serviceProvider jako IServiceProvider
        /// serviceProvider as IServiceProvider
        /// </param>
        public static async Task RunMigrationAsync<T>(IServiceProvider serviceProvider) where T : DbContext
        {
            try
            {
                using (IServiceScope serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (T dbContext = serviceScope.ServiceProvider.GetService<T>())
                    {
                        if (null != dbContext)
                        {
                            log4net.Debug($"Check Migrate");
                            if ((await dbContext.Database.GetPendingMigrationsAsync().ConfigureAwait(false)).Any())
                            {
                                log4net.Debug($"Migrate");
                                await dbContext.Database.MigrateAsync().ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
            }
        }
        #endregion
    }
    #endregion
}