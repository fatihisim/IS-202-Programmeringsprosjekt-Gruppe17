using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IS202.NrlApp.Data;
using IS202.NrlApp.Models;

namespace IS202.NrlApp.Controllers
{
    /// <summary>
    /// Controller som håndterer alle operasjoner relatert til luftfartshindringer.
    /// Inkluderer registrering, visning, godkjenning/avvisning og redigering av rapporter.
    /// </summary>
    public class ObstacleController : Controller
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Konstruktør som injiserer databasekontekst.
        /// </summary>
        public ObstacleController(AppDbContext db) => _db = db;

        /// <summary>
        /// Returnerer nåværende tidspunkt i norsk tidssone (CET/CEST).
        /// Konverterer fra UTC til Europa/Oslo tidssone.
        /// </summary>
        private DateTime GetNorwegianTime()
        {
            var norwegianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, norwegianTimeZone);
        }

        /// <summary>
        /// Viser alle hindringer med mulighet for filtrering (tilgjengelig for alle).
        /// </summary>
        [HttpGet]
        public IActionResult List(string statusFilter = "", string typeFilter = "")
        {
            // Hent ALLE rapporter for statistikk
            var allReports = _db.Obstacles.ToList();
            
            // Beregn statistikk fra ALLE rapporter (ufiltrert)
            ViewBag.TotalCount = allReports.Count;
            ViewBag.PendingCount = allReports.Count(o => o.Status == "Pending");
            ViewBag.ApprovedCount = allReports.Count(o => o.Status == "Approved");
            ViewBag.RejectedCount = allReports.Count(o => o.Status == "Rejected");

            // Deretter filtrer for tabellen
            var query = allReports.AsQueryable();

            // Filtrer etter status
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                query = query.Where(o => o.Status == statusFilter);
            }

            // Filtrer etter type
            if (!string.IsNullOrEmpty(typeFilter) && typeFilter != "All")
            {
                query = query.Where(o => o.ObstacleType == typeFilter);
            }

            var filteredReports = query.OrderByDescending(o => o.CreatedAt).ToList();

            // Send filtervalg til view
            ViewBag.StatusFilter = statusFilter;
            ViewBag.TypeFilter = typeFilter;

            return View(filteredReports);
        }

        /// <summary>
        /// Viser bare brukerens egne hindringer (krever innlogging).
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult MyReports(string statusFilter = "", string typeFilter = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Hent ALLE brukerens rapporter først for statistikk
            var allMyReports = _db.Obstacles.Where(o => o.UserId == userId).ToList();
            
            // Beregn statistikk fra ALLE rapporter (ufiltrert)
            ViewBag.TotalCount = allMyReports.Count;
            ViewBag.PendingCount = allMyReports.Count(o => o.Status == "Pending");
            ViewBag.ApprovedCount = allMyReports.Count(o => o.Status == "Approved");
            ViewBag.RejectedCount = allMyReports.Count(o => o.Status == "Rejected");

            // Deretter filtrer for tabellen
            var query = allMyReports.AsQueryable();

            // Filtrer etter status
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                query = query.Where(o => o.Status == statusFilter);
            }

            // Filtrer etter type
            if (!string.IsNullOrEmpty(typeFilter) && typeFilter != "All")
            {
                query = query.Where(o => o.ObstacleType == typeFilter);
            }

            var filteredReports = query.OrderByDescending(o => o.CreatedAt).ToList();

            // Send filtervalg til view
            ViewBag.StatusFilter = statusFilter;
            ViewBag.TypeFilter = typeFilter;

            return View(filteredReports);
        }

        /// <summary>
        /// Dashboard for Registerfører - viser alle rapporter med behandlingsverktøy.
        /// Kun tilgjengelig for Registerfører og Admin.
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult Dashboard(string statusFilter = "", string typeFilter = "")
        {
            var userRole = User.FindFirst("Role")?.Value;
            
            // Sjekker om brukeren er Registerfører eller Admin
            if (userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "You do not have permission to access the Dashboard.";
                return RedirectToAction("Index", "Home");
            }

            // Henter ALLE rapporter for statistikk (uavhengig av filter)
            var allObstacles = _db.Obstacles.ToList();
            
            // Beregner statistikk fra ALLE rapporter
            ViewBag.TotalCount = allObstacles.Count;
            ViewBag.PendingCount = allObstacles.Count(o => o.Status == "Pending");
            ViewBag.ApprovedCount = allObstacles.Count(o => o.Status == "Approved");
            ViewBag.RejectedCount = allObstacles.Count(o => o.Status == "Rejected");

            // Henter filtrerte rapporter for tabell og kart
            var query = _db.Obstacles.AsQueryable();

            // Filtrer etter status
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                query = query.Where(o => o.Status == statusFilter);
            }

            // Filtrer etter type
            if (!string.IsNullOrEmpty(typeFilter) && typeFilter != "All")
            {
                query = query.Where(o => o.ObstacleType == typeFilter);
            }

            // Sorter etter status (Pending → Rejected → Approved) og deretter CreatedAt
            var filteredObstacles = query
                .OrderBy(o => o.Status == "Pending" ? 0 : o.Status == "Rejected" ? 1 : 2)
                .ThenByDescending(o => o.CreatedAt)
                .ToList();

            // Send filtervalg til view
            ViewBag.StatusFilter = statusFilter;
            ViewBag.TypeFilter = typeFilter;

            return View(filteredObstacles);
        }

        /// <summary>
        /// Godkjenner en rapport (bare Registerfører og Admin).
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string? feedback)
        {
            var userRole = User.FindFirst("Role")?.Value;
            
            if (userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "You do not have permission to approve reports.";
                return RedirectToAction(nameof(Dashboard));
            }

            var obstacle = _db.Obstacles.Find(id);
            
            if (obstacle == null)
            {
                TempData["Error"] = "Obstacle not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            obstacle.Status = "Approved";
            obstacle.ProcessedBy = User.Identity?.Name;
            obstacle.ProcessedAt = GetNorwegianTime();
            obstacle.Feedback = feedback;

            _db.SaveChanges();

            TempData["Success"] = $"Report #{id} approved successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        /// <summary>
        /// Avviser en rapport (bare Registerfører og Admin).
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string? feedback)
        {
            var userRole = User.FindFirst("Role")?.Value;
            
            if (userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "You do not have permission to reject reports.";
                return RedirectToAction(nameof(Dashboard));
            }

            var obstacle = _db.Obstacles.Find(id);
            
            if (obstacle == null)
            {
                TempData["Error"] = "Obstacle not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            obstacle.Status = "Rejected";
            obstacle.ProcessedBy = User.Identity?.Name;
            obstacle.ProcessedAt = GetNorwegianTime();
            obstacle.Feedback = feedback;

            _db.SaveChanges();

            TempData["Success"] = $"Report #{id} rejected successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        /// <summary>
        /// Viser rapporterings-skjema med interaktivt kart (krever innlogging).
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult DataForm()
        {
            return View(new ObstacleData());
        }

        /// <summary>
        /// Mottar og lagrer en ny hindring fra pilot (krever innlogging).
        /// Brukerinformasjon hentes automatisk fra claims.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DataForm([Bind("ObstacleType,Comment,Latitude,Longitude,GeometryType,GeoJsonData")] ObstacleData model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Henter bruker-informasjon automatisk fra claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue("FullName") ?? User.Identity?.Name ?? "Unknown User";
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
                CreatedAt    = GetNorwegianTime()
            };

            _db.Obstacles.Add(entity);
            _db.SaveChanges();

            TempData["Success"] = "Obstacle successfully registered and awaiting review.";

            return RedirectToAction(nameof(MyReports));
        }

        /// <summary>
        /// Fullskjerm kartvisning av alle hindringer.
        /// </summary>
        [HttpGet]
        public IActionResult Overview()
        {
            var data = _db.Obstacles
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(data);
        }

        /// <summary>
        /// Viser redigeringsskjema for en hindring (krever innlogging).
        /// Sjekker tillatelser basert på eierskap og rolle.
        /// </summary>
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

            // Piloter kan ikke redigere godkjente rapporter
            if (obstacle.Status == "Approved" && obstacle.UserId == userId && userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "Cannot edit approved reports. Contact NRL if changes are needed.";
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
            ViewBag.CurrentStatus = obstacle.Status;
            return View(model);
        }

        /// <summary>
        /// Håndterer redigering av en hindring (krever innlogging).
        /// Pilot-redigering av avvist rapport setter status tilbake til Pending.
        /// </summary>
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

            // Piloter kan ikke redigere godkjente rapporter
            if (obstacle.Status == "Approved" && obstacle.UserId == userId && userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "Cannot edit approved reports. Contact NRL if changes are needed.";
                return RedirectToAction(nameof(MyReports));
            }

            // Oppdaterer feltene
            obstacle.ObstacleType = model.ObstacleType;
            obstacle.Comment = model.Comment;
            obstacle.Latitude = model.Latitude;
            obstacle.Longitude = model.Longitude;
            obstacle.GeometryType = model.GeometryType;
            obstacle.GeoJsonData = model.GeoJsonData;

            // Hvis pilot redigerer avvist rapport, sett status til Pending for ny gjennomgang
            if (obstacle.Status == "Rejected" && obstacle.UserId == userId && userRole != "Registerfører" && userRole != "Admin")
            {
                obstacle.Status = "Pending";
                obstacle.ProcessedBy = null;
                obstacle.ProcessedAt = null;
                obstacle.Feedback = null;
                TempData["Success"] = "Report updated and resubmitted for review.";
            }
            // Hvis Registerfører/Admin redigerer, sett status til Approved
            else if (userRole == "Registerfører" || userRole == "Admin")
            {
                if (obstacle.Status == "Pending" || obstacle.Status == "Rejected")
                {
                    obstacle.Status = "Approved";
                    obstacle.ProcessedBy = User.Identity?.Name;
                    obstacle.ProcessedAt = GetNorwegianTime();
                }
                TempData["Success"] = "Report updated and approved successfully.";
            }
            else
            {
                TempData["Success"] = "Obstacle report updated successfully.";
            }

            _db.SaveChanges();
            
            // Rollebasert redirect
            if (userRole == "Registerfører" || userRole == "Admin")
            {
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                return RedirectToAction(nameof(MyReports));
            }
        }

        /// <summary>
        /// Sletter en hindring (krever innlogging).
        /// Piloter kan ikke slette godkjente rapporter.
        /// </summary>
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

            // Piloter kan ikke slette godkjente rapporter
            if (obstacle.Status == "Approved" && obstacle.UserId == userId && userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "Cannot delete approved reports. Contact NRL if removal is needed.";
                
                if (userRole == "Registerfører" || userRole == "Admin")
                    return RedirectToAction(nameof(Dashboard));
                else
                    return RedirectToAction(nameof(MyReports));
            }

            _db.Obstacles.Remove(obstacle);
            _db.SaveChanges();

            TempData["Success"] = "Obstacle report deleted successfully.";
            
            // Rollebasert redirect
            if (userRole == "Registerfører" || userRole == "Admin")
            {
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                return RedirectToAction(nameof(MyReports));
            }
        }
    }
}