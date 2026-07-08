namespace VillaAgency.Business.Constants
{
    /// <summary>
    /// Contains centralized cache key definitions.
    ///
    /// Currently retained for future caching implementations.
    /// Product caching was replaced with server-side pagination,
    /// therefore these keys are not actively used.
    /// </summary>
    public static class CacheKeys
    {
        public const string ProductsUiCacheKey = "products_ui_cache_key";
    }
}
