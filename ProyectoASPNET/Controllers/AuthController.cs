using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

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
            if (HttpContext.User.Identity.IsAuthenticated)
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
                HttpContext.Session.SetObject("TIPOUSER", usuario.TipoUsuario);
                ClaimsIdentity identity = new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role);
                Claim claimName = new Claim(ClaimTypes.Name, usuario.IdUsuario.ToString());
                Claim claimRole = new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString());
                identity.AddClaim(claimName);
                identity.AddClaim(claimRole);
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
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
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("CheckRoutes");
            }
            usuario.Direccion += ", " + provincia;
            Usuario user = await this.repo.RegisterUsuarioAsync(usuario, contrasenya);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TIPOUSER");
            HttpContext.Session.Remove("CESTA");
            HttpContext.Session.Remove("RESTAURANTE");
            return RedirectToAction("Index", "Home");
        }
    }
}
