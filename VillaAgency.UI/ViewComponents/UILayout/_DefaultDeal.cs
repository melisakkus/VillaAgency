using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.ViewComponents.UILayout
{
    public class _DefaultDeal(IProductService _productService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Categories = await _productService.TGetUniqueCategoriesAsync();
            var values = await _productService.TGetRandomProductPerCategoryAsync();
            return View(values);
        }
    }
}
