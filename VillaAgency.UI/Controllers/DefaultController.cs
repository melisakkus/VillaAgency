using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.MessageDtos;

namespace VillaAgency.WebUI.Controllers
{
    public class DefaultController(IMessageService _messageService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                            );
                return Json(new { success = false, errors = errors });
            }

            await _messageService.TCreateAsync(dto);
            return Json(new { success = true, message = "Your message has been sent successfully!" });
        }
    }
}
