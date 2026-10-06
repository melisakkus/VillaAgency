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
        public async Task<IActionResult> SendMessage(CreateMessageDto dto, string? website, long? formLoadedAt)
        {
            // Spam bot check: hidden field filled or form submitted too fast → pretend success, don't save
            var tooFast = formLoadedAt is null ||
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds() - formLoadedAt < 3;
            if (!string.IsNullOrEmpty(website) || tooFast)
                return Json(new { success = true, message = "Your message has been sent successfully!" });

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
