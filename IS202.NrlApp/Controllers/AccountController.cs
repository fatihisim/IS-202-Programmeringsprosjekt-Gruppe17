using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IS202.NrlApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IS202.NrlApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Yönlendirer til startsiden hvor login-skjemaet finnes
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

        // Håndterer innlogging (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Welcome back! You have successfully logged in.";
                    return RedirectToAction("Index", "Home");
                }

                // Login mislyktes
                TempData["Error"] = "Invalid email or password. Please try again.";
                
                // Debug: logger feilen
                Console.WriteLine($"Login failed for: {model.Email}");
            }
            else
            {
                TempData["Error"] = "Please fill in all required fields.";
            }

            return RedirectToAction("Index", "Home");
        }

        // Viser registreringssiden
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Håndterer registrering
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
                    // Legger til claims
                    await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName));
                    await _userManager.AddClaimAsync(user, new Claim("PhoneNumber", model.PhoneNumber ?? ""));
                    await _userManager.AddClaimAsync(user, new Claim("Role", model.Role));
                    await _userManager.AddClaimAsync(user, new Claim("Organization", model.Organization ?? ""));

                    // Logger inn automatisk
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Success"] = $"Welcome, {model.FullName}! Your account has been created successfully.";

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // Logger ut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}