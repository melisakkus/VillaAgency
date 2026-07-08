namespace VillaAgency.Business.Abstract
{
    /// <summary>
    /// Defines a generic caching contract.
    ///
    /// Although the current application relies on pagination instead of caching,
    /// this abstraction is preserved to support future scalability scenarios
    /// (e.g. IMemoryCache or Redis) without affecting higher application layers.
    /// </summary>
    public interface ICacheService
    {
        bool TryGet<T>(string cacheKey, out T value);
        void Set<T>(string cacheKey, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration);
        void Remove(string cacheKey);
    }
}
