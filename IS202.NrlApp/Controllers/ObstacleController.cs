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
    /// Controller for alle operasjoner knyttet til rapportering og håndtering av luftfartshindringer. 
    /// Inkluderer visning, filtrering, innsending, godkjenning, redigering og sletting av rapporter. 
    /// </summary>
    public class ObstacleController : Controller
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initialiserer controlleren med databasekontekst via dependency injection. 
        /// </summary>
        public ObstacleController(AppDbContext db) => _db = db;

        /// <summary>
        /// Henter nåværende tidspunkt i norsk tidssone (CET/CEST)
        /// </summary>
        private DateTime GetNorwegianTime()
        {
            var norwegianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, norwegianTimeZone);
        }

        /// <summary>
        /// Viser alle hindringer med mulighet for filtrering etter status og type. 
        /// Tilgjengelig for alle brukere. 
        /// </summary>
        [HttpGet]
        public IActionResult List(string statusFilter = "", string typeFilter = "")
        {
            // Henter alle rapporter ufiltrert for statistikkvisning
            var allReports = _db.Obstacles.ToList();
            
            // Statistikk (ufiltrert) 
            ViewBag.TotalCount = allReports.Count;
            ViewBag.PendingCount = allReports.Count(o => o.Status == "Pending");
            ViewBag.ApprovedCount = allReports.Count(o => o.Status == "Approved");
            ViewBag.RejectedCount = allReports.Count(o => o.Status == "Rejected");

            // Deretter filtreres kun tabellvisningen 
            var query = allReports.AsQueryable();

            // Filtrer på status 
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                query = query.Where(o => o.Status == statusFilter);
            }

            // Filtrer på type 
            if (!string.IsNullOrEmpty(typeFilter) && typeFilter != "All")
            {
                query = query.Where(o => o.ObstacleType == typeFilter);
            }
            /// Sortere nyeste først 
            var filteredReports = query.OrderByDescending(o => o.CreatedAt).ToList();

            // Sender valgte filter ti View
            ViewBag.StatusFilter = statusFilter;
            ViewBag.TypeFilter = typeFilter;

            return View(filteredReports);
        }

        /// <summary>
        /// Viser kun innlogget brukers egne rapporter. 
        /// Støtter filtrering etter status og type. 
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult MyReports(string statusFilter = "", string typeFilter = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Hent alle rapporter brukeren har sendt inn (før filtrering)
            var allMyReports = _db.Obstacles.Where(o => o.UserId == userId).ToList();
            
            // Statistikk til dashboardet 
            ViewBag.TotalCount = allMyReports.Count;
            ViewBag.PendingCount = allMyReports.Count(o => o.Status == "Pending");
            ViewBag.ApprovedCount = allMyReports.Count(o => o.Status == "Approved");
            ViewBag.RejectedCount = allMyReports.Count(o => o.Status == "Rejected");

            
            var query = allMyReports.AsQueryable();

            //Filtrer etter status 
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
        /// Dashboard for Registerfører og Admin. 
        /// Viser alle rapporter og gir verktøy for behandling (approve/reject)
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult Dashboard(string statusFilter = "", string typeFilter = "")
        {
            var userRole = User.FindFirst("Role")?.Value;
            
            // Sjekker tilgang 
            if (userRole != "Registerfører" && userRole != "Admin")
            {
                TempData["Error"] = "You do not have permission to access the Dashboard.";
                return RedirectToAction("Index", "Home");
            }

            // Henter alle rapporter (før filtrering) for statistikk 
            var allObstacles = _db.Obstacles.ToList();
            
    
            ViewBag.TotalCount = allObstacles.Count;
            ViewBag.PendingCount = allObstacles.Count(o => o.Status == "Pending");
            ViewBag.ApprovedCount = allObstacles.Count(o => o.Status == "Approved");
            ViewBag.RejectedCount = allObstacles.Count(o => o.Status == "Rejected");

            // Filtrer tabell og kart 
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

            // Sorterer: Pending - rejected - approved 
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
        /// Godkjenner en rapport. 
        /// Kun registerfører eller Admin. 
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string? feedback)
        {
            var userRole = User.FindFirst("Role")?.Value;
            /// Tilgangssjekk 
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
             /// Oppdaterer status og metadata 
            obstacle.Status = "Approved";
            obstacle.ProcessedBy = User.FindFirstValue("FullName") ?? User.Identity?.Name;
            obstacle.ProcessedAt = GetNorwegianTime();
            obstacle.Feedback = feedback;

            _db.SaveChanges();

            TempData["Success"] = $"Report #{id} approved successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        /// <summary>
        /// Avviser en rapport. 
        /// Kun Registerfører. 
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
            obstacle.ProcessedBy = User.FindFirstValue("FullName") ?? User.Identity?.Name;
            obstacle.ProcessedAt = GetNorwegianTime();
            obstacle.Feedback = feedback;

            _db.SaveChanges();

            TempData["Success"] = $"Report #{id} rejected successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        /// <summary>
        /// Viser rapporteringsskjema der brukeren kan registrere en ny hindring. 
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult DataForm()
        {
            return View(new ObstacleData());
        }

        /// <summary>
        /// Lagrer ny innsendt hindringsrapport. 
        /// Henter brukerdata fra Claims. 
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DataForm([Bind("ObstacleType,Comment,Latitude,Longitude,GeometryType,GeoJsonData")] ObstacleData model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Henter brukerinformasjo fra claims
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
        /// Viser alle hindringer i kartvisning. 
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
        /// Viser redigeringsskjema for en valgt hindringsrapport. 
        /// Sjekker at brukeren har riktig rolle eller eier rapporten. 
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

            // Sjekker tilgang 
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

            // Oppretter ViewModel 
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
        /// Behandler redigering av hindringsrapport. 
        /// Rolle og eierskapssjekk styrer hvilke handlinger som er tillatt. 
        /// Piloter som redigerer en avvist rapport setter statusen tilbake til Pending. 
        /// Registerfører/Admin godkjenner autimatisk endringen. 
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

            // Henter brukerrolle og eierskap 
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
                    obstacle.ProcessedBy = User.FindFirstValue("FullName") ?? User.Identity?.Name;
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
        /// Sletter en hindringsrapport. Kun eier, Regiserfører eller Admin kan slette. 
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
            /// utfører sletting 
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