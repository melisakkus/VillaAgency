using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using VillaAgency.Business.Abstract;
using VillaAgency.Business.Constants;
using VillaAgency.Dto.ProductDtos;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var products = await _productService.TGetListAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            await _productService.TCreateAsync(dto);
            _cacheService.Remove(CacheKeys.ProductsUiCacheKey);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(ObjectId id)
        {
            await _productService.TDeleteAsync(id);
            _cacheService.Remove(CacheKeys.ProductsUiCacheKey);
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
            _cacheService.Remove(CacheKeys.ProductsUiCacheKey);
            return RedirectToAction(nameof(Index));
        }

    }
}
