using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Models;
using ProyectoASPNET.Services;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class AuthController : Controller
    {
        private IServiceRestaurantes service;

        public AuthController(IServiceRestaurantes service)
        {
            this.service = service;
        }

        public IActionResult CheckRoutes()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                int tipoUsuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.Role).Value);
                if (tipoUsuario == 1)
                    return RedirectToAction("Index", "Restaurantes");
                else if (tipoUsuario == 2)
                    return RedirectToAction("Index", "PanelAdmin");
                else if (tipoUsuario == 3)
                    return RedirectToAction("Index", "PanelRestaurante");
                else return RedirectToAction("Logout", "Auth");
            }
            else return RedirectToAction("Logout", "Auth"); // redirige al login
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
            Usuario usuario = await this.service.LoginUsuarioAsync(email, password);
            if (usuario != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role);
                Claim claimName = new Claim(ClaimTypes.Name, usuario.Correo);
                Claim claimID = new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString());
                Claim claimRole = new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString());
                identity.AddClaim(claimName);
                identity.AddClaim(claimID);
                identity.AddClaim(claimRole);
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
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
            Usuario user = await this.service.RegisterUsuarioAsync(usuario, contrasenya);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("CESTA");
            HttpContext.Session.Remove("RESTAURANTE");
            return RedirectToAction("Index", "Home");
        }
    }
}
