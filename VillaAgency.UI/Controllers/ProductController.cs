using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using System.Threading.Tasks;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.ProductDtos;

namespace VillaAgency.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "products_ui_cache_key";

        public ProductController(IProductService productService, IMemoryCache memoryCache)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        //public async Task<IActionResult> Index()
        //{
        //    var products = await _productService.TGetListAsync();
        //    return View(products);
        //}

        public async Task <IActionResult> Index()
        {
            if (!_memoryCache.TryGetValue(CacheKey, out List<ResultProductDto> cachedProducts))
            {
                cachedProducts = await _productService.TGetListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(CacheKey, cachedProducts, cacheEntryOptions);
            }
            return View(cachedProducts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            await _productService.TCreateAsync(dto);
            _memoryCache.Remove(CacheKey);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(ObjectId id)
        {
            await _productService.TDeleteAsync(id);
            _memoryCache.Remove(CacheKey);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(ObjectId id)
        {
            var value = await _productService.TGetByIdAsync(id);
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductDto dto)
        {
            await _productService.TUpdateAsync(dto);
            _memoryCache.Remove(CacheKey);
            return RedirectToAction(nameof(Index));
        }

    }
}
