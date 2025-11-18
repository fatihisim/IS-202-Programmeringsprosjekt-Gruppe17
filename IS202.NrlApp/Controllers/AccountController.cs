using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IS202.NrlApp.Models;
using IS202.NrlApp.Data;
using System.Security.Claims;

namespace IS202.NrlApp.Controllers
{
    /// <summary>
    /// Controller som håndterer brukerautentisering (registrering, innlogging, utlogging).
    /// Bruker ASP.NET Core Identity for sikker passordbehandling og sesjoner.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _db;

        /// <summary>
        /// Konstruktør som injiserer UserManager, SignInManager og databasekontekst.
        /// </summary>
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        /// <summary>
        /// Viser innloggingssiden (GET).
        /// Hvis brukeren allerede er logget inn, omdirigeres de til startsiden.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            // Hvis brukeren allerede er logget inn, send til hjem
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            
            // Send til startsiden hvor login-skjemaet er
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Håndterer innlogging (POST).
        /// Validerer brukernavn og passord, og sjekker om brukeren har nye tilbakemeldinger.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // Sjekker om brukeren har nye tilbakemeldinger fra NRL-behandlere
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var reportsWithFeedback = _db.Obstacles
                            .Where(o => o.UserId == user.Id && !string.IsNullOrEmpty(o.Feedback))
                            .Count();
                        
                        if (reportsWithFeedback > 0)
                        {
                            TempData["Info"] = $"You have {reportsWithFeedback} report(s) with feedback from NRL Officers.";
                        }
                        else
                        {
                            TempData["Success"] = "Logged in successfully.";
                        }
                    }
                    
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password");
            }

            // Hvis innlogging feiler, send tilbake til startsiden
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Viser registreringssiden (GET).
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Håndterer brukerregistrering (POST).
        /// Oppretter ny bruker, legger til claims (FullName, Role, Organization, PhoneNumber),
        /// og logger brukeren inn automatisk.
        /// Omdirigerer basert på brukerens rolle etter vellykket registrering.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Legger til claims for tilleggsinformasjon om brukeren
                    await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName));
                    await _userManager.AddClaimAsync(user, new Claim("PhoneNumber", model.PhoneNumber ?? ""));
                    await _userManager.AddClaimAsync(user, new Claim("Role", model.Role));
                    await _userManager.AddClaimAsync(user, new Claim("Organization", model.Organization ?? ""));

                    // Logger inn brukeren automatisk etter registrering
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Success"] = "Account created successfully! Welcome to NRL Obstacle Reporting.";

                    // Rollebasert omdirigering etter registrering
                    if (model.Role == "Registerfører" || model.Role == "Admin")
                    {
                        // Registerfører og Admin sendes til Dashboard
                        return RedirectToAction("Dashboard", "Obstacle");
                    }
                    else
                    {
                        // Pilot sendes til MyReports
                        return RedirectToAction("MyReports", "Obstacle");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Logger ut brukeren og omdirigerer til startsiden.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}