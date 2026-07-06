using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.ViewComponents.UILayout
{
    public class _DefaultBanner(IBannerService _bannerService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _bannerService.TGetListAsync();
            return View(values);
        }
    }
}
