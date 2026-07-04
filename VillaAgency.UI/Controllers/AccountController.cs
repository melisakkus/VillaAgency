using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.AuthDtos;

namespace VillaAgency.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService? _authService;

        public AccountController(IAuthService? authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _authService.LoginAsync(dto);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard",new {area="Admin"});
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", "Your account is not active. Please contact the administrator.");
                return View(dto);
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(dto);
        }

       
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}

