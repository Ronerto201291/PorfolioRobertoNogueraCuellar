using Microsoft.Extensions.Caching.Memory;
using PruebaAngular.Infrastructure.Data.Core;
using System;

namespace PruebaAngular.Infrastructure.Data.Cache
{
    public class InMemmoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public InMemmoryCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
            }
            _cache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }
    }
}
