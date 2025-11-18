using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IS202.NrlApp.Models;
using IS202.NrlApp.Data;
using System.Security.Claims;

namespace IS202.NrlApp.Controllers
{
    /// <summary>
    /// Controller som håndterer all brukerautentiserig:
    /// registrering, innlogging og utlogging. 
    /// Benytter ASP.NET Core Identity for sikker håndtering av passord og sesjoner. 
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _db;

        /// <summary>
        /// Konstruktør som mottar UserManager, SignInManager, AppDbContext via dependency injection.  
        /// </summary>
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        /// <summary>
        /// Viser innloggingssiden (GET).
        /// Dersom brukeren allerede er innlogget, sendes de til forsiden. 
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            // Hvis brukeren allerede er innlogget, send videre til hjemmesiden
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            
            // Vis forsiden der innloggingsskjemaet er plassert 
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Håndterer forsøk på innlogging (POST).
        /// Validerer brukernavn og passord, og sjekker om brukeren har tilbakemeldinger.
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
                    // Henter bruker og sjekket om det finnes nye tilbakemeldinger på rapporter
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
               /// Feil brukernavn eller passord 
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
        /// Oppretter en ny bruker, legger til nødvendige claims (FullName, Role, Organization, PhoneNumber),
        /// logger brukeren inn automatisk, og omdirigerer basert på brukerens rolle. 
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
                    // Legger til ekstra brukerdata som claims 
                    await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName));
                    await _userManager.AddClaimAsync(user, new Claim("PhoneNumber", model.PhoneNumber ?? ""));
                    await _userManager.AddClaimAsync(user, new Claim("Role", model.Role));
                    await _userManager.AddClaimAsync(user, new Claim("Organization", model.Organization ?? ""));

                    // Logger inn den nye brukeren 
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Success"] = "Account created successfully! Welcome to NRL Obstacle Reporting.";

                    // Rollebasert navigasjon etter registrering 
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
               /// viser eventuelle valideringsfei fra Identity 
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Logger ut brukeren og sender dem tilbake til forsiden. 
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