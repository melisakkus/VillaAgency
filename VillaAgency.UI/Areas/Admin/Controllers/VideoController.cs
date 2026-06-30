using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.FeatureDtos;
using VillaAgency.Dto.VideoViewDtos;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VideoController : Controller
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _videoService.TGetListAsync();
            return View(values);
        }

        public async Task<IActionResult> MakeItActive(string id)
        {
            await _videoService.TMakeItActive(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MakeItPassive(string id)
        {
            await _videoService.TMakeItPassive(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _videoService.TDeleteAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateVideoViewDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _videoService.TCreateAsync(dto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string id)
        {
            var value = await _videoService.TGetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateVideoViewDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _videoService.TUpdateAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
