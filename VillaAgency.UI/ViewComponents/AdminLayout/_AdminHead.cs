using Microsoft.AspNetCore.Mvc;

namespace VillaAgency.WebUI.ViewComponents.AdminLayout
{
    public class _AdminHead : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
