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

        public IActionResult CheckRoutes()
        {
            if (HttpContext.Session.GetString("USER") == null)
                return RedirectToAction("Login", "Auth");
            int tipoUsuario = HttpContext.Session.GetObject<int>("TIPOUSER");
            if (tipoUsuario == 1)
                return RedirectToAction("Index", "Restaurantes");
            else if (tipoUsuario == 2)
                return RedirectToAction("Index", "PanelAdmin");
            else
                return RedirectToAction("Index", "PanelRestaurante");
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("USER") != null)
            {
                return RedirectToAction("CheckRoutes");
            }
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
                return RedirectToAction("CheckRoutes");
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
        public async Task<IActionResult> Register(Usuario usuario, string contrasenya, string provincia)
        {
            if (HttpContext.Session.GetString("USER") != null)
            {
                return RedirectToAction("CheckRoutes");
            }
            usuario.Direccion += ", " + provincia;
            Usuario user = await this.repo.RegisterUsuarioAsync(usuario, contrasenya);
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
