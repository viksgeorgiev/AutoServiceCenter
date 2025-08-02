using Microsoft.AspNetCore.Authorization;

namespace AutoServiceCenter.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using ViewModels;

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            var viewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            int statusCode = HttpContext.Response.StatusCode;

            if (statusCode == 404)
            {
                return View("NotFound", viewModel);
            }

            return View("Error", viewModel);
        }

        [AllowAnonymous]
        [Route("/Error/404")]
        public new IActionResult NotFound()
        {
            var viewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View("NotFound", viewModel);
        }
    }
}
