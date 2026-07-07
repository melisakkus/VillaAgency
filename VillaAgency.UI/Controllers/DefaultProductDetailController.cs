using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.Controllers
{
    public class DefaultProductDetailController(IProductService _productService) : Controller
    {
        public async Task<IActionResult> Index(string id)
        {
            var value = await _productService.TGetByIdAsync(id);
            return View(value);
        }
    }
}
