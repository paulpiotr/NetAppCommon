#region using

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#endregion

#nullable enable annotations

#region namespace

namespace NetAppCommon.Helpers;

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
    private static readonly ILog Log4Net =
        Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

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
            using IServiceScope serviceScope =
                serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            await using TDbContext? context = serviceScope.ServiceProvider.GetService<TDbContext>();
            if (null != context)
            {
                await RunMigrationAsync(context);
            }
        }
        catch (Exception e)
        {
            Log4Net.Error(e);
            if (null != e.InnerException)
            {
                Log4Net.Error(e.InnerException);
            }
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
            try
            {
                await DatabaseMssqlMdf.GetInstance(context.Database?.GetDbConnection()?.ConnectionString)
                    .CreateAsync();
            }
            catch (Exception e)
            {
                Log4Net.Warn(e);
                if (null != e.InnerException)
                {
                    Log4Net.Warn(e.InnerException);
                }
            }

            var isPendingMigrations =
                (await (context.Database ?? throw new InvalidOperationException()).GetPendingMigrationsAsync())
                .Any();
            if (isPendingMigrations)
            {
                try
                {
                    await context.Database.MigrateAsync();
                }
                catch (Exception e)
                {
                    Log4Net.Warn(e);
                    if (null != e.InnerException)
                    {
                        Log4Net.Warn(e.InnerException);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log4Net.Error(e);
            if (null != e.InnerException)
            {
                Log4Net.Error(e.InnerException);
            }
        }
    }

    #endregion
}

#endregion

#endregion
