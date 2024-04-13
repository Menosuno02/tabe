using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Filters;
using ProyectoASPNET.Models;
using ProyectoASPNET.Services;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class UsuarioController : Controller
    {
        private IServiceRestaurantes service;

        public UsuarioController(IServiceRestaurantes service)
        {
            this.service = service;
        }

        [AuthorizeUser]
        public async Task<IActionResult> Perfil()
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Usuario usu = await this.service.FindUsuarioAsync(idusuario);
            return View(usu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> Perfil(Usuario usuario)
        {
            await this.service.EditUsuarioAsync(usuario);
            return RedirectToAction("Perfil");
        }

        [AuthorizeUser]
        public async Task<IActionResult> Pedidos()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Pedido> pedidos = await this.service.GetPedidosUsuarioAsync();
            List<int> idPedidos = pedidos.Select(p => p.IdPedido).ToList();
            ViewData["PRODUCTOS"] = await this.service.GetProductosPedidoViewAsync(idPedidos);
            return View(pedidos);
        }

        [AuthorizeUser]
        public async Task<IActionResult> ModificarContrasenya()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> ModificarContrasenya
            (string actual, string nueva, string confirmar)
        {
            if (nueva != confirmar)
                ViewData["CASO"] = "1";
            else
            {
                int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Usuario usu = await this.service.FindUsuarioAsync(idusuario);
                if (!await this.service.ModificarContrasenyaAsync(usu, actual, nueva))
                    ViewData["CASO"] = "2";
                else ViewData["CASO"] = "3";
            }
            return View();
        }
    }
}
