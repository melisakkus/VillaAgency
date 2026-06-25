using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.BannerDtos;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private readonly IBannerService _bannerService;
        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _bannerService.TGetListAsync();
            return View(values);
        }

        public IActionResult CreateBanner()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBanner(CreateBannerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _bannerService.TCreateAsync(dto);
            return RedirectToAction("Index");   
        }


        public async Task<IActionResult> DeleteBanner(string id) 
        {
            await _bannerService.TDeleteAsync(id);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> UpdateBanner(string id)
        {
            var dto = await _bannerService.TGetByIdAsync(id);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBanner(UpdateBannerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _bannerService.TUpdateAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
