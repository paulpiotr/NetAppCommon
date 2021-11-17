#region using

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAppCommon.Extensions.Caching.Distributed;

#endregion

namespace NetAppCommon.Extensions.DependencyInjection;

/// <summary>
///     Extension methods for setting up memory cache related services in an <see cref="IServiceCollection" />.
/// </summary>
public static class MemoryCacheServiceCollectionExtensions
{
    /// <summary>
    ///     Is added
    /// </summary>
    private static bool _isAdded;

    /// <summary>
    ///     Check public static IServiceCollection AddNetAppCommonDistributedMemoryCache is added as Singleton
    /// </summary>
    /// <returns>
    ///     True if IServiceCollection AddNetAppCommonDistributedMemoryCache is run, else false
    /// </returns>
    public static bool IsAdded() => _isAdded;

    /// <summary>
    ///     Adds a default implementation of <see cref="DistributedCache" /> that stores items in memory
    ///     to the <see cref="IServiceCollection" />. Frameworks that require a distributed cache to work
    ///     can safely add this dependency as part of their dependency list to ensure that there is at least
    ///     one implementation available.
    /// </summary>
    /// <remarks>
    ///     <see cref="AddDistributedMemoryCache(IServiceCollection)" /> should only be used in single
    ///     server scenarios as this cache stores items in memory and doesn't expand across multiple machines.
    ///     For those scenarios it is recommended to use a proper distributed cache that can expand across
    ///     multiple machines.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddNetAppCommonDistributedMemoryCache(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddOptions();
        //services.TryAdd(ServiceDescriptor.Singleton<IDistributedCache, MemoryDistributedCache>());
        services.TryAdd(ServiceDescriptor.Singleton<ICommonDistributedCache, CommonMemoryDistributedCache>());
        _isAdded = true;

        return services;
    }
}
