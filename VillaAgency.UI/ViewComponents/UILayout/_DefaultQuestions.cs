using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.ViewComponents.UILayout
{
    public class _DefaultQuestions(IQuestionService _questionService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _questionService.TGetListAsync();
            return View(values);
        }
    }
}
