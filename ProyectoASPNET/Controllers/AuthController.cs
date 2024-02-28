using Microsoft.AspNetCore.Mvc;

namespace ProyectoASPNET.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Object user)
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
