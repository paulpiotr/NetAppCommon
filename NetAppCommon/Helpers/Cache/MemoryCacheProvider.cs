#region using

using System;
using Microsoft.Extensions.Caching.Memory;
using Polly.Caching;
using Polly.Utilities;

#endregion

namespace NetAppCommon.Helpers.Cache
{
    public class MemoryCacheProvider
    {
        private const double MemoryCacheTimeSpan = 1000;
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        private readonly IMemoryCache _cache;

        private readonly TimeSpan _memoryCachetimeSpan = TimeSpan.FromSeconds(MemoryCacheTimeSpan);

        public MemoryCacheProvider()
        {
        }

        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            _cache ??= memoryCache;
        }

        private IMemoryCache GetMemoryCache() => _cache ?? Cache;

        public static MemoryCacheProvider GetInstance(IMemoryCache memoryCache) => new(memoryCache);

        public static MemoryCacheProvider GetInstance() => new();

        public (bool hit, object value) TryGet(string key)
        {
            var cacheHit = GetMemoryCache().TryGetValue(key, out object value);
            return (cacheHit, value);
        }

        public void Put(string key, object value, TimeSpan timeSpan) => Put(key, value, new Ttl(timeSpan));

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
            (var hit, object value) = TryGet(key);
            if (hit)
            {
                return value;
            }

            return null;
        }

        public object Get(string key, object @object, double? timeSpan)
        {
            (var hit, object value) = TryGet(key);
            if (hit)
            {
                return value;
            }

            Put(key, @object, TimeSpan.FromMilliseconds(timeSpan ?? MemoryCacheTimeSpan));
            return @object;
        }

        public object Get(string key, object @object, Ttl ttl)
        {
            (var hit, object value) = TryGet(key);
            if (hit)
            {
                return value;
            }

            Put(key, @object, ttl);
            return @object;
        }
    }
}
