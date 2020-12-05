using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        private static readonly log4net.ILog Log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
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
#if DEBUG 
                            Log4net.Debug($"Check Migrate { dbContext.Database.GetDbConnection().ConnectionString }");
#endif
                            if ((await dbContext.Database.GetPendingMigrationsAsync().ConfigureAwait(false)).Any())
                            {
#if DEBUG 
                                Log4net.Debug($"Migrate { dbContext.Database.GetDbConnection().ConnectionString }");
#endif
                                await dbContext.Database.MigrateAsync().ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
            }
        }
#endregion
    }
#endregion
}
