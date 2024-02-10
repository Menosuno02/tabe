using Microsoft.AspNetCore.Mvc;

namespace ProyectoASPNET.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
