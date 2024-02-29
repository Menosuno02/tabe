using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class AuthController : Controller
    {
        private RepositoryRestaurantes repo;

        public AuthController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            UsuarioView usuario = await this.repo.LoginUsuario(email, password);
            if (usuario != null)
            {
                HttpContext.Session.SetObject("USER", usuario.IdUsuario);
                return RedirectToAction("Restaurantes", "Index");
            }
            else
            {
                ViewData["MENSAJE"] = "Error";
                return View();
            }

        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UsuarioView usuario)
        {
            return RedirectToAction("Login");
        }
    }
}
