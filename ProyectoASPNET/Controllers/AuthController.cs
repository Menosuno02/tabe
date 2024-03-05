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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            Usuario usuario = await this.repo.LoginUsuarioAsync(email, password);
            if (usuario != null)
            {
                HttpContext.Session.SetObject("USER", usuario.IdUsuario);
                HttpContext.Session.SetObject("TIPOUSER", usuario.TipoUsuario);
                return RedirectToAction("Index", "Restaurantes");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string contrasenya, Usuario usuario)
        {
            Usuario user = await this.repo.RegisterUsuarioAsync(contrasenya, usuario);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("USER");
            HttpContext.Session.Remove("TIPOUSER");
            HttpContext.Session.Remove("CESTA");
            return RedirectToAction("Index", "Home");
        }
    }
}
