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

        // Viser bare brukerens egne hindringer (krever innlogging)
        [HttpGet]
        [Authorize]
        public IActionResult MyReports()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var myObstacles = _db.Obstacles
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(myObstacles);
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

        // Viser redigeringsskjema for en hindring (krever innlogging)
        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var obstacle = _db.Obstacles.Find(id);
            
            if (obstacle == null)
            {
                TempData["Error"] = "Obstacle not found.";
                return RedirectToAction(nameof(MyReports));
            }

            // Sjekker om brukeren eier rapporten eller er Registerfører/Admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst("Role")?.Value;
            
            if (obstacle.UserId != userId && userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "You do not have permission to edit this report.";
                return RedirectToAction(nameof(MyReports));
            }

            // Konverterer til ViewModel
            var model = new ObstacleData
            {
                ObstacleType = obstacle.ObstacleType,
                Comment = obstacle.Comment,
                Latitude = obstacle.Latitude,
                Longitude = obstacle.Longitude,
                GeometryType = obstacle.GeometryType,
                GeoJsonData = obstacle.GeoJsonData
            };

            ViewBag.ObstacleId = id;
            return View(model);
        }

        // Håndterer redigering av en hindring (krever innlogging)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ObstacleType,Comment,Latitude,Longitude,GeometryType,GeoJsonData")] ObstacleData model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ObstacleId = id;
                return View(model);
            }

            var obstacle = _db.Obstacles.Find(id);
            
            if (obstacle == null)
            {
                TempData["Error"] = "Obstacle not found.";
                return RedirectToAction(nameof(MyReports));
            }

            // Sjekker tillatelse
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst("Role")?.Value;
            
            if (obstacle.UserId != userId && userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "You do not have permission to edit this report.";
                return RedirectToAction(nameof(MyReports));
            }

            // Oppdaterer feltene
            obstacle.ObstacleType = model.ObstacleType;
            obstacle.Comment = model.Comment;
            obstacle.Latitude = model.Latitude;
            obstacle.Longitude = model.Longitude;
            obstacle.GeometryType = model.GeometryType;
            obstacle.GeoJsonData = model.GeoJsonData;

            _db.SaveChanges();

            TempData["Success"] = "Obstacle report updated successfully.";
            return RedirectToAction(nameof(MyReports));
        }

        // Sletter en hindring (krever innlogging)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var obstacle = _db.Obstacles.Find(id);
            
            if (obstacle == null)
            {
                TempData["Error"] = "Obstacle not found.";
                return RedirectToAction(nameof(MyReports));
            }

            // Sjekker tillatelse
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst("Role")?.Value;
            
            if (obstacle.UserId != userId && userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "You do not have permission to delete this report.";
                return RedirectToAction(nameof(MyReports));
            }

            _db.Obstacles.Remove(obstacle);
            _db.SaveChanges();

            TempData["Success"] = "Obstacle report deleted successfully.";
            return RedirectToAction(nameof(MyReports));
        }
    }
}