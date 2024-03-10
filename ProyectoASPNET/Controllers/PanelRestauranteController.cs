using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class PanelRestauranteController : Controller
    {
        private RepositoryRestaurantes repo;
        private HelperCesta helperCesta;

        public PanelRestauranteController
            (RepositoryRestaurantes repo, HelperCesta helperCesta)
        {
            this.repo = repo;
            this.helperCesta = helperCesta;
        }

        public IActionResult Index(string? nomvista)
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            if (nomvista != null)
            {
                ViewData["NOMVISTA"] = nomvista;
            }
            return View();
        }

        public async Task<IActionResult> _PedidosRestaurante()
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = HttpContext.Session.GetObject<int>("USER");
            List<Pedido> pedidos = await this.repo.GetPedidosRestauranteAsync(idusuario);
            List<int> idPedidos = pedidos.Select(p => p.IdPedido).ToList();
            ViewData["PRODUCTOS"] = await this.repo.GetProductosPedidoViewAsync(idPedidos);
            return PartialView("_PedidosRestaurante", pedidos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _PedidosRestaurante(int idpedido, int estado)
        {
            await this.repo.UpdateEstadoPedido(idpedido, estado);
            return RedirectToAction("Index", new { nomvista = "_PedidosRestaurante" });
        }

        public async Task<IActionResult> _EditRestaurante()
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = HttpContext.Session.GetObject<int>("USER");
            Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(idusuario);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantesAsync();
            return PartialView("_EditRestaurante", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditRestaurante(Restaurante rest, IFormFile fileimagen)
        {
            await this.repo.EditRestauranteAsync(rest, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_EditRestaurante" });
        }

        public async Task<IActionResult> _ProductosRestaurante()
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Producto> productos;
            int idusuario = HttpContext.Session.GetObject<int>("USER");
            Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(idusuario);
            productos = await this.repo.GetProductosRestauranteAsync(rest.IdRestaurante);
            ViewData["IDRESTAURANTE"] = rest.IdRestaurante;
            return PartialView("_ProductosRestaurante", productos);
        }

        public async Task<IActionResult> _CreateProducto()
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = HttpContext.Session.GetObject<int>("USER");
            Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(idusuario);
            return PartialView("_CreateProducto", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _CreateProducto(Producto prod, IFormFile fileimagen)
        {
            await this.repo.CreateProductoAsync(prod, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }

        public async Task<IActionResult> _DetailsProducto(int idprod)
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.repo.FindProductoAsync(idprod);
            return PartialView("_DetailsProducto", prod);
        }

        public async Task<IActionResult> _EditProducto(int idprod)
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.repo.FindProductoAsync(idprod);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasProductosAsync();
            return PartialView("_EditProducto", prod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditProducto(Producto prod, int[] categproducto, IFormFile fileimagen)
        {
            await this.repo.EditProductoAsync(prod, categproducto, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }

        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.repo.DeleteProductoAsync(id);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }
    }
}
