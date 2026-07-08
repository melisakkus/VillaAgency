using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VillaAgency.Business.Abstract;

namespace VillaAgency.Business.Concrete
{
    /// <summary>
    /// Provides a generic caching abstraction for the Business layer.
    ///
    /// Design Note:
    /// This service was implemented during the initial architecture phase to
    /// improve performance by caching product data in memory and reducing
    /// repetitive database queries.
    ///
    /// During later development, the application was redesigned to use
    /// server-side pagination and filtering. Since each request retrieves
    /// only a limited subset of records, the performance benefit of caching
    /// became minimal while introducing additional cache invalidation
    /// complexity.
    ///
    /// Therefore, the caching mechanism is currently inactive.
    /// The abstraction has intentionally been preserved because it allows
    /// the application to adopt IMemoryCache, Redis, or any distributed
    /// caching provider in the future without changing the business layer.
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