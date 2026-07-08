using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VillaAgency.WebUI.Models;

namespace VillaAgency.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var userMessage = "This operation cannot be performed at the moment. Please try again later.";

            if (exceptionFeature?.Error != null)
            {
                var ex = exceptionFeature.Error;

                _logger.LogError(ex,
                    "Unhandled exception while processing {Path}: {Message}",
                    exceptionFeature.Path, ex.Message);
            }

            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                UserMessage = userMessage
            };

            return View(model);
        }
    }
}
