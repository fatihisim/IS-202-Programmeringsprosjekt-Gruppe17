using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IS202.NrlApp.Models;

namespace IS202.NrlApp.Controllers
{
    /// <summary>
    /// Controller som håndterer hovedsiden, personvernsiden og feilmeldinger.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Konstruktør som mottar logger for å logge hendelser og feil.
        /// </summary>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Viser startsiden (Index).
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Viser personvernsiden (Privacy).
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Viser feilmeldingssiden dersom en feil oppstår.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
