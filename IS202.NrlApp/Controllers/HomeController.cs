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
    /// Controller for hovedsiden, personvernsiden og feilhåndteringer. 
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        /// <summary>
        /// Initaliserer Homecontroller med logger og databasekontekst via dependency injection.
        /// </summary>
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Viser startsiden (Index) og henter antall registrerte hindringer fra databasen.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Henter antall registrerte hindringer i databasen. 
            int obstacleCount = await _context.Obstacles.CountAsync();

            // Gjør verdien tilgjengelig i View via ViewBag
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
        /// Debug side som viser alle claims og roller til innlogget bruker. 
        /// Brukes for å teste at autentisering og roller fungerer som forventet. 
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
        /// Henter alle claims som en liste for visning i View 
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
        /// Viser feilmeldingssiden dersom en uventet feil oppstår i applikasjonen. 
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
