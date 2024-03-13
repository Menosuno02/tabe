using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Filters;
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

        [AuthorizeUser]
        public IActionResult Index(string? nomvista)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            if (nomvista != null)
            {
                ViewData["NOMVISTA"] = nomvista;
            }
            return View();
        }

        [AuthorizeUser]
        public async Task<IActionResult> _PedidosRestaurante()
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = int.Parse(HttpContext.User.Identity.Name);
            List<Pedido> pedidos = await this.repo.GetPedidosRestauranteAsync(idusuario);
            List<int> idPedidos = pedidos.Select(p => p.IdPedido).ToList();
            ViewData["PRODUCTOS"] = await this.repo.GetProductosPedidoViewAsync(idPedidos);
            return PartialView("_PedidosRestaurante", pedidos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _PedidosRestaurante(int idpedido, int estado)
        {
            await this.repo.UpdateEstadoPedidoAsync(idpedido, estado);
            return RedirectToAction("Index", new { nomvista = "_PedidosRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _EditRestaurante()
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = int.Parse(HttpContext.User.Identity.Name);
            Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(idusuario);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantesAsync();
            return PartialView("_EditRestaurante", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _EditRestaurante(Restaurante rest, IFormFile fileimagen)
        {
            await this.repo.EditRestauranteAsync(rest, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_EditRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _ProductosRestaurante()
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Producto> productos;
            int idusuario = int.Parse(HttpContext.User.Identity.Name);
            Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(idusuario);
            productos = await this.repo.GetProductosRestauranteAsync(rest.IdRestaurante);
            ViewData["IDRESTAURANTE"] = rest.IdRestaurante;
            return PartialView("_ProductosRestaurante", productos);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _CreateProducto()
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = int.Parse(HttpContext.User.Identity.Name);
            Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(idusuario);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasProductosAsync(rest.IdRestaurante);
            return PartialView("_CreateProducto", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _CreateProducto(Producto prod, int[] categproducto, IFormFile fileimagen)
        {
            await this.repo.CreateProductoAsync(prod, categproducto, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _DetailsProducto(int idprod)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.repo.FindProductoAsync(idprod);
            return PartialView("_DetailsProducto", prod);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _EditProducto(int idprod)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.repo.FindProductoAsync(idprod);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasProductosAsync(prod.IdRestaurante);
            return PartialView("_EditProducto", prod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _EditProducto(Producto prod, int[] categproducto, IFormFile fileimagen)
        {
            await this.repo.EditProductoAsync(prod, categproducto, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.repo.DeleteProductoAsync(id);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _CategoriasRestaurante()
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            int idusuario = int.Parse(HttpContext.User.Identity.Name);
            Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(idusuario);
            List<CategoriaProducto> categorias = await this.repo.GetCategoriasProductosAsync(rest.IdRestaurante);
            ViewData["IDRESTAURANTE"] = rest.IdRestaurante;
            return PartialView("_CategoriasRestaurante", categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _CategoriasRestaurante(string categoria)
        {
            int idusuario = int.Parse(HttpContext.User.Identity.Name);
            Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(idusuario);
            CategoriaProducto categProducto = await this.repo.CreateCategoriaProductoAsync(rest.IdRestaurante, categoria);
            return RedirectToAction("Index", new { nomvista = "_CategoriasRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> DeleteCategoriaProducto(int idcategoria)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 3)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.repo.DeleteCategoriaProductoAsync(idcategoria);
            return RedirectToAction("Index", new { nomvista = "_CategoriasRestaurante" });
        }
    }
}
