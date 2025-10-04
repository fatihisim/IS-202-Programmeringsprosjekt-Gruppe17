using IS202.NrlApp.Data;
using IS202.NrlApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace IS202.NrlApp.Controllers
{
    /// <summary>
    /// Controller som håndterer opprettelse, visning og lagring av hindringsdata.
    /// </summary>
    public class ObstacleController : Controller
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Konstruktør som initialiserer databasekonteksten.
        /// </summary>
        public ObstacleController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Viser skjemaet for å registrere ny hindring (GET).
        /// </summary>
        [HttpGet]
        public IActionResult DataForm()
        {
            return View(new ObstacleData());
        }

        /// <summary>
        /// Behandler innsending av skjema (POST). Validerer og lagrer data.
        /// </summary>
        [HttpPost]
        public IActionResult DataForm(ObstacleData model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Map ViewModel -> Entity
            var entity = new Obstacle
            {
                ReporterName = model.ReporterName,
                Organization = model.Organization,
                ObstacleType = model.ObstacleType,
                Comment = model.Comment,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };

            _db.Obstacles.Add(entity);
            _db.SaveChanges();

            // Etter lagring vises oversiktssiden
            return RedirectToAction(nameof(Overview), model);
        }

        /// <summary>
        /// Viser oversiktsside med informasjon om rapportert hindring.
        /// </summary>
        [HttpGet]
        public IActionResult Overview(ObstacleData model)
        {
            return View(model);
        }

        /// <summary>
        /// Viser liste over alle registrerte hindringer fra databasen.
        /// </summary>
        [HttpGet]
        public IActionResult List()
        {
            var items = _db.Obstacles
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(items);
        }
    }
}
