using FeedbackAPI.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace FeedbackAPI.Infrastructure
{
    public class CacheManager : ICacheManager
    {


        private readonly IMemoryCache _cache;
        private readonly IOptions<AppSettings> _settings;

        public CacheManager(IMemoryCache cache, IOptions<AppSettings> settings)
        {
            _cache = cache;
            _settings = settings;
        }

        public T GetFromCache<T>(string key) where T : class
        {
            _cache.TryGetValue(key, out T cachedResponse);
            return cachedResponse as T;
        }

        public void SetCache<T>(string key, T value) where T : class
        {
            SetCache(key, value, DateTimeOffset.Now.AddSeconds(_settings.Value.CacheSeconds));
        }

        public void SetCache<T>(string key, T value, DateTimeOffset duration) where T : class
        {
            _cache.Set(key, value, duration);
        }

        public void SetCache<T>(string key, T value, MemoryCacheEntryOptions options) where T : class
        {
            _cache.Set(key, value, options);
        }

        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }

    }
}
