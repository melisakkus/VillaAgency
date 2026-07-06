using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.ViewComponents.UILayout
{
    public class _DefaultProducts(IProductService _productService): ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _productService.TGetRandomProductPerCategoryAsync(2);
            return View(values);
        }
    }
}
