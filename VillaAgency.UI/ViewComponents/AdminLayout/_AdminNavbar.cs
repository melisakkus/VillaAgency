using Microsoft.AspNetCore.Mvc;

namespace VillaAgency.WebUI.ViewComponents.AdminLayout
{
    public class _AdminNavbar : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
