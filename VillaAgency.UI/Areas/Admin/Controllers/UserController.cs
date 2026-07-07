using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.AuthDtos;
using VillaAgency.Entity.Identity;
using VillaAgency.Entity.Identity.Constants;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UserController : AdminBaseController
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IAuthService authService, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var list = new List<(AppUser User, string Role)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                list.Add((user, roles.FirstOrDefault() ?? "-"));
            }

            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _authService.RegisterAsync(dto, Roles.Manager);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                    {
                        ModelState.AddModelError("Password", error.Description);
                    }
                    else if (error.Code.Contains("UserName"))
                    {
                        ModelState.AddModelError("UserName", error.Description);
                    }
                    else if (error.Code.Contains("Email"))
                    {
                        ModelState.AddModelError("Email", error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                return View(dto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _authService.DeleteUserAsync(id);
            ViewBag.Notification = result.Succeeded
                    ? "success|User deleted successfully."
                    : "error|An error occurred while deleting the user.";

            return RedirectToAction("Index");
        }
    }
}
