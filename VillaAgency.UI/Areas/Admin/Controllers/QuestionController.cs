using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.QuestionDtos;
using VillaAgency.Entity.Identity.Constants;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class QuestionController : AdminBaseController
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _questionService.TGetListAsync();
            return View(values);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _questionService.TDeleteAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _questionService.TCreateAsync(dto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string id)
        {
            var value = await _questionService.TGetByIdAsync(id);
            return View(value);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateQuestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _questionService.TUpdateAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
