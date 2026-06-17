using Microsoft.AspNetCore.Mvc;

namespace VillaAgency.WebUI.ViewComponents.AdminLayout
{
    public class _AdminFooter : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
