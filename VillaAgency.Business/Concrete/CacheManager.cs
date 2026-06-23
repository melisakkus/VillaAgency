using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VillaAgency.Business.Abstract;

namespace VillaAgency.Business.Concrete
{
    /// <summary>
    /// Implements In-Memory caching operations.
    /// Abstracted via 'ICacheService' to decouple the presentation layer (Controllers) from the specific caching infrastructure (Loose Coupling).
    /// This design allows seamless migration to distributed caching systems like Redis without modifying existing controller logic (Open/Closed Principle), 
    /// while centralizing cross-cutting concerns (DRY).
    /// </summary>
    public class CacheManager : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheManager> _logger;

        public CacheManager(IMemoryCache memoryCache, ILogger<CacheManager> logger)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger;
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
            _logger.LogInformation("Cache Removed: {CacheKey}", cacheKey);
        }

        public void Set<T>(string cacheKey, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };

            _memoryCache.Set(cacheKey, value, cacheOptions);

            _logger.LogDebug(
                "Cache SET: {CacheKey}, AbsoluteExpiration: {Absolute}, SlidingExpiration: {Sliding}",
                cacheKey,
                absoluteExpiration,
                slidingExpiration
            );
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            if (_memoryCache.TryGetValue(cacheKey, out value))
            {
                return true;
            }
            else
            {
                _logger.LogInformation("Cache Miss: {CacheKey}", cacheKey);
                return false;
            }
        }
    }
}