using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IS202.NrlApp.Data;
using IS202.NrlApp.Models;

namespace IS202.NrlApp.Controllers
{
    public class ObstacleController : Controller
    {
        private readonly AppDbContext _db;

        public ObstacleController(AppDbContext db) => _db = db;

        // Viser alle hindringer (tilgjengelig for alle)
        [HttpGet]
        public IActionResult List()
        {
            var items = _db.Obstacles
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(items);
        }

        // Viser rapporterings-skjema (krever innlogging)
        [HttpGet]
        [Authorize]
        public IActionResult DataForm()
        {
            return View(new ObstacleData());
        }

        // Mottar rapport fra bruker (krever innlogging)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DataForm([Bind("ObstacleType,Comment,Latitude,Longitude,GeometryType,GeoJsonData")] ObstacleData model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Henter bruker-informasjon automatisk
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity?.Name ?? "Unknown User";
            var organization = User.FindFirstValue("Organization") ?? "Not specified";

            var entity = new Obstacle
            {
                ReporterName = userName,
                Organization = organization,
                ObstacleType = model.ObstacleType,
                Comment      = model.Comment,
                Latitude     = model.Latitude,
                Longitude    = model.Longitude,
                GeometryType = model.GeometryType,
                GeoJsonData  = model.GeoJsonData,
                UserId       = userId,
                Status       = "Pending",
                CreatedAt    = DateTime.UtcNow
            };

            _db.Obstacles.Add(entity);
            _db.SaveChanges();

            TempData["Success"] = "Obstacle successfully registered and awaiting review.";

            return RedirectToAction(nameof(List));
        }

        // Fullskjerm kartvisning
        [HttpGet]
        public IActionResult Overview()
        {
            var data = _db.Obstacles
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(data);
        }
    }
}