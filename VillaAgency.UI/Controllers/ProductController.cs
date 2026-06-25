using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService, ICacheService cacheService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public async Task<IActionResult> Index()
        {
            var value = await _productService.TGetListAsync();
            return View(value);
        }
    }
}