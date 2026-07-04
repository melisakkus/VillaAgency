using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.ContactDtos;
using VillaAgency.Entity.Identity.Constants;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class ContactController : AdminBaseController
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _contactService.TGetAllAsync();
            return View(values);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContactDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _contactService.TCreateAsync(dto);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(string id)
        {
            await _contactService.TDeleteAsync(id);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Update(string id)
        {
            var dto = await _contactService.TGetByIdAsync(id);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateContactDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _contactService.TUpdateAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
