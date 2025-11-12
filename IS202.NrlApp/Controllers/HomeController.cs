using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IS202.NrlApp.Models;
using IS202.NrlApp.Data;

namespace IS202.NrlApp.Controllers
{
    /// <summary>
    /// Controller som håndterer hovedsiden, personvernsiden og feilmeldinger.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        /// <summary>
        /// Konstruktør som mottar logger og databasekontekst.
        /// </summary>
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Viser startsiden (Index) med dynamisk antall hindre.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Teller antall hindringer (obstacles) i databasen
            int obstacleCount = await _context.Obstacles.CountAsync();

            // Sender antallet til View via ViewBag
            ViewBag.ObstacleCount = obstacleCount;

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
        /// Test-side for å sjekke brukerens claims og roller.
        /// Brukes til å verifisere at rolle-systemet fungerer korrekt.
        /// </summary>
        [Authorize]
        public IActionResult TestRoles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.Identity?.Name;
            var fullName = User.FindFirstValue("FullName");
            var role = User.FindFirstValue("Role");
            var organization = User.FindFirstValue("Organization");
            var phoneNumber = User.FindFirstValue("PhoneNumber");
            
            var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            ViewBag.UserId = userId;
            ViewBag.Email = email;
            ViewBag.FullName = fullName;
            ViewBag.Role = role;
            ViewBag.Organization = organization;
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.AllClaims = allClaims;

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
