using Microsoft.AspNetCore.Mvc;

namespace ProyectoASPNET.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("USER") != null)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            return View();
        }
    }
}
