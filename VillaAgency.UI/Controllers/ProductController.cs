using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Business.Constants;
using VillaAgency.Dto.ProductDtos;

namespace VillaAgency.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICacheService _cacheService;

        public ProductController(IProductService productService, ICacheService cacheService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<IActionResult> Index()
        {
            if (!_cacheService.TryGet(CacheKeys.ProductsUiCacheKey, out List<ResultProductDto> cachedProducts))
            {
                cachedProducts = await _productService.TGetListAsync();

                _cacheService.Set(CacheKeys.ProductsUiCacheKey, cachedProducts,TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(10));
            }
            return View(cachedProducts);
        }

    }
}


//[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
//public async Task<IActionResult> Index()
//{
//    var products = await _productService.TGetListAsync();
//    return View(products);
//}
