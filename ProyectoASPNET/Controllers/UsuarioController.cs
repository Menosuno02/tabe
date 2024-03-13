using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Filters;
using ProyectoASPNET.Models;

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
            int idusuario = int.Parse(HttpContext.User.Identity.Name);
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
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 1)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = int.Parse(HttpContext.User.Identity.Name);
            List<Pedido> pedidos = await this.repo.GetPedidosUsuarioAsync(idusuario);
            List<int> idPedidos = pedidos.Select(p => p.IdPedido).ToList();
            ViewData["PRODUCTOS"] = await this.repo.GetProductosPedidoViewAsync(idPedidos);
            return View(pedidos);
        }
    }
}
