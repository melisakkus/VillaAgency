using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.CounterDtos;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ICounterService _counterService;

        public DashboardController(ICounterService counterService)
        {
            _counterService = counterService;
        }

        public async Task<IActionResult> Index()
        {
            var activeProductsTask = _counterService.TGetActiveProductsCountAsync();
            var soldProductsTask = _counterService.TGetSoldProductsCountAsync();
            var allProductsTask = _counterService.TGetAllProductsCountAsync();

            await Task.WhenAll(activeProductsTask, soldProductsTask, allProductsTask);

            var dto = new DashboardCounterDto
            {
                ActiveProductsCount = await activeProductsTask,
                SoldProductsCount = await soldProductsTask,
                TotalProductsCount = await allProductsTask
            };

            return View(dto);
        }
    }
}
