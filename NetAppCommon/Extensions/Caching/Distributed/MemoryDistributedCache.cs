#region using

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

#endregion

namespace NetAppCommon.Extensions.Caching.Distributed
{
    public class MemoryDistributedCache : Microsoft.Extensions.Caching.Distributed.MemoryDistributedCache,
        IDistributedCache
    {
        /// <summary>
        ///     Constructor of MemoryDistributedCache
        /// </summary>
        /// <param name="optionsAccessor">
        ///     IOptions <see cref="MemoryDistributedCacheOptions" /> optionsAccessor
        /// </param>
        public MemoryDistributedCache(IOptions<MemoryDistributedCacheOptions> optionsAccessor)
            : base(optionsAccessor, NullLoggerFactory.Instance)
        {
        }

        /// <summary>
        ///     Constructor of MemoryDistributedCache
        /// </summary>
        /// <param name="optionsAccessor">
        ///     IOptions <see cref="MemoryDistributedCacheOptions" /> optionsAccessor
        /// </param>
        /// <param name="loggerFactory">
        ///     ILoggerFactory loggerFactory <see cref="ILoggerFactory" />
        /// </param>
        public MemoryDistributedCache(IOptions<MemoryDistributedCacheOptions> optionsAccessor,
            ILoggerFactory loggerFactory) : base(optionsAccessor, loggerFactory)
        {
        }

        public object Get<TValue>(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = Get(key);
            if (null != value)
            {
                return JsonConvert.DeserializeObject<TValue>(Encoding.UTF8.GetString(value));
            }

            return null;
        }

        public Task<object> GetAsync<TValue>(string key, CancellationToken token = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return Task.FromResult(Get<TValue>(key));
        }

        public object Set<TValue>(string key, object value, DistributedCacheEntryOptions options)
        {
            Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, Formatting.Indented)), options);
            return Get(key);
        }

        public async Task<object> SetAsync<TValue>(string key, object value, DistributedCacheEntryOptions options,
            CancellationToken token = default)
        {
            await SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, Formatting.Indented)),
                options, token);
            return await GetAsync<TValue>(key, token);
        }
    }
}