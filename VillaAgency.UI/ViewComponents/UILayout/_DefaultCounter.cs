using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.ViewComponents.UILayout
{
    public class _DefaultCounter(ICounterService _counterService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var value = await _counterService.TGetProductCountsByCategoryAsync();
            return View(value);
        }
    }
}
