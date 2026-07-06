using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.ViewComponents.UILayout
{
    public class _DefaultFeature(IFeatureService _featureService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _featureService.TGetFilteredListAsync(x=>x.IsActive == true,1,1);
            return View(values);
        }
    }
}
