#region using

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace NetAppCommon.Helpers
{
    #region public class EntityContextHelper

    /// <summary>
    ///     Entity Context Helper
    ///     Pomocnik kontekstu jednostki
    /// </summary>
    public class EntityContextHelper
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     private readonly ILog _log4Net
        /// </summary>
        private static readonly ILog Log4net =
            Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public static async Task RunMigrationAsync<T>(IServiceProvider serviceProvider) where T : DbContext

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task RunMigrationAsync<TDbContext>(IServiceProvider serviceProvider)
            where TDbContext : DbContext
        {
            try
            {
                using (IServiceScope serviceScope =
                    serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (TDbContext context = serviceScope.ServiceProvider.GetService<TDbContext>())
                    {
                        await RunMigrationAsync(context);
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }
        }

        #endregion

        #region public static async Task RunMigrationAsync<T>(T context) where T : DbContext

        /// <summary>
        ///     Uruchom migrację bazy danych asynchronicznie
        ///     Run database migration asynchronously
        /// </summary>
        /// <typeparam name="TDbContext">
        ///     TDbContext : DbContext
        ///     TDbContext : DbContext
        /// </typeparam>
        /// <param name="context">
        ///     TDbContext context where TDbContext : DbContext
        ///     TDbContext context where TDbContext : DbContext
        /// </param>
        /// <returns>
        ///     async Task
        ///     async Task
        /// </returns>
        public static async Task RunMigrationAsync<TDbContext>(TDbContext context) where TDbContext : DbContext
        {
            try
            {
                //#if DEBUG
                //                Log4net.Debug($"RunMigrationAsync { context?.Database?.GetDbConnection()?.ConnectionString }");
                //#endif
                if (null != context)
                {
                    //#if DEBUG
                    //                    Log4net.Debug($"context { context?.Database?.GetDbConnection()?.ConnectionString }");
                    //#endif
                    try
                    {
                        //#if DEBUG
                        //                        Log4net.Debug($"Try MDF Create Async DatabaseMssqlMdf.GetInstance(context?.Database?.GetDbConnection()?.ConnectionString ).CreateAsync()...");
                        //#endif
                        await DatabaseMssqlMdf.GetInstance(context?.Database?.GetDbConnection()?.ConnectionString)
                            .CreateAsync();
                        //#if DEBUG
                        //                        Log4net.Debug($"Ok");
                        //#endif
                    }
                    catch (Exception e)
                    {
                        Log4net.Warn(
                            $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                    }

                    if ((await (context.Database ?? throw new InvalidOperationException()).GetPendingMigrationsAsync())
                        .Any())
                    {
                        //#if DEBUG
                        //                        Log4net.Debug($"Migrate { context?.Database?.GetDbConnection()?.ConnectionString }");
                        //#endif
                        try
                        {
                            //#if DEBUG
                            //                            Log4net.Debug($"Try Migrate Async context.Database.MigrateAsync()...");
                            //#endif
                            await context.Database.MigrateAsync();
                            //#if DEBUG
                            //                            Log4net.Debug($"Ok");
                            //#endif
                        }
                        catch (Exception e)
                        {
                            Log4net.Warn(
                                $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log4net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }
        }

        #endregion
    }

    #endregion
}
