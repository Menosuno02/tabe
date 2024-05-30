using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TabeNuget;
using ProyectoASPNET.Services;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class AuthController : Controller
    {
        private IServiceRestaurantes service;
        private ServiceCacheRedis serviceRedis;

        public AuthController(IServiceRestaurantes service, ServiceCacheRedis serviceRedis)
        {
            this.service = service;
            this.serviceRedis = serviceRedis;
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

        public IActionResult Login(bool? passwordchanged)
        {

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("CheckRoutes");
            }

            if (passwordchanged != null)
                ViewData["MENSAJE"] = "Contraseña";
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
                identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Correo));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString()));
#pragma warning disable CS8604 // Possible null reference argument.
                identity.AddClaim(new Claim("TOKEN", HttpContext.Session.GetString("TOKEN")));
#pragma warning restore CS8604 // Possible null reference argument.
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                HttpContext.Session.Remove("TOKEN");
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

        public async Task<IActionResult> Logout(bool? passwordchanged)
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            await this.serviceRedis.ResetCesta();
            if (passwordchanged != null)
                return RedirectToAction("Login", "Auth", new { passwordchanged = true });
            return RedirectToAction("Index", "Home");
        }
    }
}
