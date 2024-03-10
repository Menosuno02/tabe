using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("USER") != null)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            return View();
        }
    }
}
