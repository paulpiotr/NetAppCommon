#region using

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

#endregion

namespace NetAppCommon.Extensions.Caching.Distributed;

/// <summary>
///     Represents a extended distributed cache of serialized values.
/// </summary>
public interface ICommonDistributedCache : IDistributedCache
{
    /// <summary>
    ///     Gets a TValue value with the given key.
    /// </summary>
    /// <typeparam name="TValue">
    ///     Type of value
    /// </typeparam>
    /// <param name="key">
    ///     A string identifying the requested value.
    /// </param>
    /// <returns>
    ///     The located value as TValue or null.
    /// </returns>
    object Get<TValue>(string key);

    /// <summary>
    ///     Gets a TValue value with the given key async.
    /// </summary>
    /// <typeparam name="TValue">
    ///     Type of value
    /// </typeparam>
    /// <param name="key">
    ///     A string identifying the requested value.
    /// </param>
    /// <param name="token"></param>
    /// <returns>
    ///     The located value as TValue or null.
    /// </returns>
    Task<object> GetAsync<TValue>(string key, CancellationToken token = default);

    /// <summary>
    ///     Sets a object value as TValue with the given key.
    /// </summary>
    /// <typeparam name="TValue">
    ///     Type of value
    /// </typeparam>
    /// <param name="key">
    ///     A string identifying the requested value.
    /// </param>
    /// <param name="value">
    ///     The object value as TValue to set in the cache.
    /// </param>
    /// <param name="options">
    ///     The cache options for the value.
    /// </param>
    void Set<TValue>(string key, TValue value, DistributedCacheEntryOptions options = default);

    ///// <summary>
    ///// Sets a value with the given key.
    ///// </summary>
    ///// <param name="key">A string identifying the requested value.</param>
    ///// <param name="value">The value to set in the cache.</param>
    ///// <param name="options">The cache options for the value.</param>
    //new void Set(string key, byte[] value, DistributedCacheEntryOptions options);

    /// <summary>
    ///     Sets a object value as TValue with the given key async.
    /// </summary>
    /// <typeparam name="TValue">
    ///     Type of value
    /// </typeparam>
    /// <param name="key">
    ///     A string identifying the requested value.
    /// </param>
    /// <param name="value">
    ///     The object value as TValue to set in the cache.
    /// </param>
    /// <param name="options">
    ///     The cache options for the value.
    /// </param>
    /// <returns>
    ///     Object value as TValue or null if not set
    /// </returns>
    Task SetAsync<TValue>(string key, TValue value, DistributedCacheEntryOptions options = default,
        CancellationToken token = default);
}
