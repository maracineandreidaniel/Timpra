using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Timpra.BusinessLogic.Services
{
    public class RedisCacheService
    {
        private readonly IDistributedCache? _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T GetCachedData<T>(string key)
        {
            var jsonData = _cache.GetString(key);   

            if(jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }

        public void SetCachedData<T>(string key, T data, TimeSpan cacheDuration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration,
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            var jsonData = JsonSerializer.Serialize(data);

            _cache.SetString(key, jsonData, options);
        }

        public void RemoveCache(string key) => _cache.Remove(key);
    }
}
