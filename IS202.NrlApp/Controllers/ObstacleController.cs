using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using IS202.NrlApp.Data;
using IS202.NrlApp.Models;

namespace IS202.NrlApp.Controllers
{
    public class ObstacleController : Controller
    {
        private readonly AppDbContext _db;

        // Konstruktør – får tilgang til databasen via dependency injection
        public ObstacleController(AppDbContext db) => _db = db;

        // GET: /Obstacle/List
        // Viser tabell med alle registrerte hinder + innebygd kart på samme side
        [HttpGet]
        public IActionResult List()
        {
            // Henter alle hinder fra databasen (nyeste først)
            var items = _db.Obstacles
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(items);
        }

        // GET: /Obstacle/DataForm
        // Viser registreringsskjemaet
        [HttpGet]
        public IActionResult DataForm()
        {
            // Tom visningsmodell til skjemaet
            return View(new ObstacleData());
        }

        // POST: /Obstacle/DataForm
        // Mottar innsending fra skjemaet (PRG-mønster brukes: Post → Redirect → Get)
        // [ValidateAntiForgeryToken] beskytter mot CSRF-angrep
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DataForm([Bind("ReporterName,Organization,ObstacleType,Comment,Latitude,Longitude")] ObstacleData model)
        {
            // Hvis valideringen feiler, vis skjemaet på nytt med feilmeldinger
            if (!ModelState.IsValid)
                return View(model);

            // Mapper fra skjema-modell (ViewModel) til database-entitet (Domain Model)
            var entity = new Obstacle
            {
                ReporterName = model.ReporterName,
                Organization = model.Organization,
                ObstacleType = model.ObstacleType,
                Comment     = model.Comment,
                Latitude    = model.Latitude,
                Longitude   = model.Longitude,
                CreatedAt   = DateTime.Now   // lagrer tidspunkt for innmelding
            };

            // Lagrer i databasen
            _db.Obstacles.Add(entity);
            _db.SaveChanges();

            // Viser grønn "suksess"-melding etter redirect
            TempData["Success"] = "Obstacle successfully registered.";

            // Redirect til liste (unngår dobbelt-post ved refresh)
            return RedirectToAction(nameof(List));
        }

        // GET: /Obstacle/Overview
        // Egen fullskjerms kartside (filtre + søk). Returnerer alle hinder som modell.
        // Brukes hvis man ønsker en dedikert kartvisning i tillegg til tabell-siden.
        [HttpGet]
        public IActionResult Overview()
        {
            var data = _db.Obstacles
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            // View: Views/Obstacle/Overview.cshtml (forventer IEnumerable<Obstacle>)
            return View(data);
        }
    }
}
