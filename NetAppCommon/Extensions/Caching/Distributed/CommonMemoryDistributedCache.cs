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
    public class CommonMemoryDistributedCache : MemoryDistributedCache,
        ICommonDistributedCache//, IMemoryCache
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

            _memoryCache ??= new MemoryCache(optionsAccessor.Value);
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

            _memoryCache ??= new MemoryCache(optionsAccessor.Value, loggerFactory);
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

            return Helpers.Object.ObjectHelper.GetDefaultValue<TValue>();
        }

        public Task<object> GetAsync<TValue>(string key, CancellationToken token = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return Task.FromResult(Get<TValue>(key));
        }

        public void Set<TValue>(string key, object value, DistributedCacheEntryOptions options = default)
        {

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            options ??= new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddYears(1),
                //AbsoluteExpirationRelativeToNow = TimeSpan.FromDays((DateTime.Now.AddYears(1) - DateTime.Now).Days),
                //SlidingExpiration = TimeSpan.FromDays((DateTime.Now.AddYears(1) - DateTime.Now).Days)
            };

            var valueAsByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, Formatting.Indented));

            if (null != _memoryCache)
            {
                var memoryCacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = options.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                    SlidingExpiration = options.SlidingExpiration,
                    Size = valueAsByte.Length
                };
                _memoryCache.Set(key, valueAsByte, memoryCacheEntryOptions);
            }

            else
            {
                Set(key, valueAsByte, options);
            }
        }

        public async Task SetAsync<TValue>(string key, object value, DistributedCacheEntryOptions options = default,
            CancellationToken token = default)
        {
            await SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, Formatting.Indented)),
                options, token);
        }
        
    }
}
