using IS202.NrlApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace IS202.NrlApp.Controllers
{
    public class ObstacleController : Controller
    {
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
                // If validation fails, show the form again
                return View(model);
            }

            // If valid, redirect to the Overview page with the submitted data
            return RedirectToAction(nameof(Overview), model);
        }

        [HttpGet]
        public IActionResult Overview(ObstacleData model)
        {
            return View(model);
        }
    }
}
