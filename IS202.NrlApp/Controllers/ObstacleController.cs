using IS202.NrlApp.Data;
using IS202.NrlApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace IS202.NrlApp.Controllers
{
    public class ObstacleController : Controller
    {
        private readonly AppDbContext _db;

        public ObstacleController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult DataForm()
        {
            return View(new ObstacleData());
        }

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

            // Redirect to Overview page
            return RedirectToAction(nameof(Overview), model);
        }

        [HttpGet]
        public IActionResult Overview(ObstacleData model)
        {
            return View(model);
        }

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
