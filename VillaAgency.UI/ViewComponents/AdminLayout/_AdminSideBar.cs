using Microsoft.AspNetCore.Mvc;

namespace VillaAgency.WebUI.ViewComponents.AdminLayout
{
    public class _AdminSideBar : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
