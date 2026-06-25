using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VillaAgency.Business.Abstract;

namespace VillaAgency.Business.Concrete
{
    /// <summary>
    /// Generic caching abstraction.
    ///
    /// NOTE:
    /// This service is currently not used in the application.
    /// During early development, In-Memory caching was implemented to reduce
    /// database access for product listings.
    ///
    /// Later, the application was redesigned to use server-side pagination
    /// and filtering. Since only a small subset of data is fetched per request,
    /// the performance gain from caching became negligible compared to the added
    /// cache invalidation complexity.
    ///
    /// The abstraction is intentionally preserved for future scalability needs,
    /// allowing an easy transition to MemoryCache, Redis, or other distributed
    /// caching providers without affecting higher application layers.
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