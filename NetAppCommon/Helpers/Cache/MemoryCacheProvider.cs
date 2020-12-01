using System;
using Microsoft.Extensions.Caching.Memory;
using Polly.Caching;
using Polly.Utilities;

namespace NetAppCommon.Helpers.Cache
{
    public class MemoryCacheProvider
    {
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        private readonly IMemoryCache _cache;

        private IMemoryCache GetMemoryCache()
        {
            return _cache ?? Cache;
        }

        public MemoryCacheProvider()
        {
        }

        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public static MemoryCacheProvider GetInstance(IMemoryCache memoryCache)
        {
            return new MemoryCacheProvider(memoryCache);
        }

        public static MemoryCacheProvider GetInstance()
        {
            return new MemoryCacheProvider();
        }

        public (bool hit, object value) TryGet(string key)
        {
            var cacheHit = GetMemoryCache().TryGetValue(key, out var value);
            return (cacheHit, value);
        }

        public void Put(string key, object value, TimeSpan timeSpan)
        {
            Put(key, value, new Ttl(timeSpan));
        }

        public void Put(string key, object value, Ttl ttl)
        {
            TimeSpan remaining = DateTimeOffset.MaxValue - SystemClock.DateTimeOffsetUtcNow();
            var options = new MemoryCacheEntryOptions();
            if (ttl.SlidingExpiration)
            {
                options.SlidingExpiration = ttl.Timespan < remaining ? ttl.Timespan : remaining;
            }
            else
            {
                if (ttl.Timespan == TimeSpan.MaxValue)
                {
                    options.AbsoluteExpiration = DateTimeOffset.MaxValue;
                }
                else
                {
                    options.AbsoluteExpirationRelativeToNow = ttl.Timespan < remaining ? ttl.Timespan : remaining;
                }
            }
            GetMemoryCache().Set(key, value, options);
        }

        public object Get(string key)
        {
            (var hit, var value) = TryGet(key);
            if (hit)
            {
                return value;
            }
            return null;
        }
    }
}
