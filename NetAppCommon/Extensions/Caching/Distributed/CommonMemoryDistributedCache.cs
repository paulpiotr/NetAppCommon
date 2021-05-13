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
using NetAppCommon.Extensions.DependencyInjection;
using NetAppCommon.Helpers.Object;
using Newtonsoft.Json;

#endregion

namespace NetAppCommon.Extensions.Caching.Distributed
{
    public class CommonMemoryDistributedCache : MemoryDistributedCache,
        ICommonDistributedCache
    {
        private static IMemoryCache _memoryCache;

        /// <summary>
        ///     Constructor of MemoryDistributedCache
        /// </summary>
        /// <param name="optionsAccessor">
        ///     IOptions <see cref="MemoryDistributedCacheOptions" /> optionsAccessor
        /// </param>
        public CommonMemoryDistributedCache(IOptions<MemoryDistributedCacheOptions> optionsAccessor)
            : base(optionsAccessor, NullLoggerFactory.Instance)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            if (!MemoryCacheServiceCollectionExtensions.IsAdded())
            {
                _memoryCache ??= new MemoryCache(optionsAccessor.Value);
            }
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
        public CommonMemoryDistributedCache(IOptions<MemoryDistributedCacheOptions> optionsAccessor,
            ILoggerFactory loggerFactory) : base(optionsAccessor, loggerFactory)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (!MemoryCacheServiceCollectionExtensions.IsAdded())
            {
                _memoryCache ??= new MemoryCache(optionsAccessor.Value, loggerFactory);
            }
        }

        public object Get<TValue>(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            byte[] value;

            if (null != _memoryCache)
            {
                _memoryCache.TryGetValue(key, out value);
            }
            else
            {
                value = Get(key);
            }

            if (null != value)
            {
                return JsonConvert.DeserializeObject<TValue>(Encoding.UTF8.GetString(value));
            }

            return ObjectHelper.GetDefaultValue<TValue>();
        }

        public async Task<object> GetAsync<TValue>(string key, CancellationToken token = default)
        {
            return await Task.Run(() => Task.FromResult(Get<TValue>(key)), token);
        }

        public void Set<TValue>(string key, TValue value, DistributedCacheEntryOptions options)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var valueAsByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, Formatting.Indented));

            if (null != _memoryCache)
            {
                var memoryCacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = options?.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = options?.AbsoluteExpirationRelativeToNow,
                    SlidingExpiration = options?.SlidingExpiration,
                    Size = valueAsByte.Length
                };

                _memoryCache.Set(key, valueAsByte, memoryCacheEntryOptions);
            }
            else
            {
                base.Set(key, valueAsByte, options);
            }
        }

        public async Task SetAsync<TValue>(string key, TValue value, DistributedCacheEntryOptions options = default,
            CancellationToken token = default)
        {
            await Task.Run(() => Set(key, value, options), token);
        }
    }
}