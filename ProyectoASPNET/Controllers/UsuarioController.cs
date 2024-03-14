using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Filters;
using ProyectoASPNET.Models;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class UsuarioController : Controller
    {
        private RepositoryRestaurantes repo;

        public UsuarioController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [AuthorizeUser]
        public async Task<IActionResult> Perfil()
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Usuario usu = await this.repo.FindUsuarioAsync(idusuario);
            return View(usu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> Perfil(Usuario usuario)
        {
            await this.repo.EditUsuarioAsync(usuario);
            return RedirectToAction("Perfil");
        }

        [AuthorizeUser]
        public async Task<IActionResult> Pedidos()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<Pedido> pedidos = await this.repo.GetPedidosUsuarioAsync(idusuario);
            List<int> idPedidos = pedidos.Select(p => p.IdPedido).ToList();
            ViewData["PRODUCTOS"] = await this.repo.GetProductosPedidoViewAsync(idPedidos);
            return View(pedidos);
        }
    }
}
