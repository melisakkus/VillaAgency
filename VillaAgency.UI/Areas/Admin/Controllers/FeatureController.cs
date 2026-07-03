using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.FeatureDtos;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeatureController : Controller
    {
        private readonly IFeatureService _featureService;

        public FeatureController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _featureService.TGetAllAsync();
            return View(values);
        }

        public async Task<IActionResult> GetActiveList()
        {
            var values =await _featureService.TGetFilteredListAsync(x=>x.IsActive==true);
            return View(values);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _featureService.TDeleteAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            var model = new CreateFeatureDto();

            model.FAQs.Add(new FAQItemDto());

            model.FeatureDetails.Add(new FeatureDetailDto
            {
                Icon = "bi bi-star"
            });

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateFeatureDto dto)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    Console.WriteLine($"KEY = {item.Key}");

                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine($"ERROR = {error.ErrorMessage}");
                    }
                }

                return View(dto);
            }
            await _featureService.TCreateAsync(dto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string id)
        {
            var value = await _featureService.TGetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateFeatureDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _featureService.TUpdateAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
