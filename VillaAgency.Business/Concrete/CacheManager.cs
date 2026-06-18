using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using VillaAgency.Business.Abstract;

namespace VillaAgency.Business.Concrete
{
    public class CacheManager : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public void Set<T>(string cacheKey, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };

            _memoryCache.Set(cacheKey, value, cacheOptions);
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            return _memoryCache.TryGetValue(cacheKey, out value);
        }
    }
}


//`ICacheService` soyutlamasını kullanmak; sunum katmanını (Controller) belirli bir teknolojiye sıkı sıkıya bağlamayarak (**Tight Coupling**) projenin bağımlılık mimarisini esnek tutmamızı sağlar. Bu yaklaşım sayesinde, ileride artan trafik yükü sebebiyle In-Memory Cache yerine merkezi bir dağıtık önbellek altyapısına (**Redis**) geçiş kararı alındığında, Controller tarafındaki tek bir satır koda dahi dokunmadan sadece ilgili servis sınıfının (`CacheManager`) içini değiştirerek tüm projeyi ölçeklememize imkan tanır (**Open/Closed Principle**). Ayrıca önbelleğe alma süreçlerine dair loglama, hata yönetimi veya Unit Test simülasyonları (Mocking) gibi kurumsal ihtiyaçların tek bir merkezden, kod tekrarı olmadan (DRY) kolayca yönetilmesini mümkün kılar.

// ### ⚡ Architectural Decision: Why `ICacheService` Abstraction Over Native `IMemoryCache`?

//Instead of tightly coupling the presentation layer by directly injecting .NET's native `IMemoryCache` into the Controllers, an abstract **`ICacheService`** wrapper has been architected. This architectural pattern delivers several enterprise-grade benefits:

//* **Loose Coupling:**The presentation layer(Controllers) is completely isolated from the underlying caching technology.Controllers interact strictly with the predefined contracts (`TryGet`, `Set`, `Remove`), remaining unaware of how or where the data is stored.
//* **Open/Closed Principle (SOLID):**If horizontal scaling demands shifting from a volatile single-server infrastructure (In-Memory) to a distributed caching solution like **Redis**, **not a single line of code in the presentation layer will be altered.** The migration can be completed seamlessly by merely injecting a new `RedisCacheManager` implementation via the IoC Container.
//* **Centralized Governance & Robustness (DRY):**Cross - cutting concerns such as logging cache hits/misses, implementing global fallback exception handling, or mocking dependencies for Unit Testing are consolidated within a single operational hub (`CacheManager`), eliminating code duplication.