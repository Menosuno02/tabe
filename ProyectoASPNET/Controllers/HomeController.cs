using Microsoft.AspNetCore.Mvc;

namespace ProyectoASPNET.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            return View();
        }
    }
}
