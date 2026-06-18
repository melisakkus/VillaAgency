namespace VillaAgency.Business.Abstract
{
    public interface ICacheService
    {
        bool TryGet<T>(string cacheKey, out T value);
        void Set<T>(string cacheKey, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration);
        void Remove(string cacheKey);
    }
}
