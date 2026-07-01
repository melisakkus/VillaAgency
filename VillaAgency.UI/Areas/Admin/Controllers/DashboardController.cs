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
            var allProductsCountTask = _counterService.TGetAllProductsCountAsync();
            var activeProductsCountTask = _counterService.TGetActiveProductsCountAsync();
            var soldProductsCountTask = _counterService.TGetSoldProductsCountAsync();
            var rentedProductsCountTask = _counterService.TGetRentedProductsCountAsync();
            var categoriesWithCountTask = _counterService.TGetProductCountsByCategoryAsync();
            var unreadMessagesCountTask = _counterService.TGetUnReadMessagesCountAsync();
            var lastMessagesTask = _counterService.TGetLastMessagesAsync(5);

            await Task.WhenAll(
                allProductsCountTask,
                activeProductsCountTask,
                soldProductsCountTask,
                rentedProductsCountTask,
                categoriesWithCountTask,
                unreadMessagesCountTask,
                lastMessagesTask
            );

            var dto = new DashboardCounterDto
            {
                AllProductsCount = allProductsCountTask.Result,
                ActiveProductsCount = activeProductsCountTask.Result,
                SoldProductsCount = soldProductsCountTask.Result,
                RentedProductsCount = rentedProductsCountTask.Result,
                UnreadMessagesCount = unreadMessagesCountTask.Result,
                CategoriesWithCount = categoriesWithCountTask.Result,
                LastMessages = lastMessagesTask.Result
            };

            return View(dto);
        }
    }
}
