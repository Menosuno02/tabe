using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProyectoASPNET.Filters;
using TabeNuget;
using ProyectoASPNET.Services;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class PanelRestauranteController : Controller
    {
        private IServiceRestaurantes service;
        private string UrlBlobProductos;

        public PanelRestauranteController(IServiceRestaurantes service, IConfiguration configuration)
        {
            this.service = service;
            UrlBlobProductos = configuration.GetValue<string>("BlobUrls:UrlProductos");
        }

        [AuthorizeUser]
        public IActionResult Index(string? nomvista)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
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
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Pedido> pedidos = await this.service.GetPedidosRestauranteAsync();
            List<int> idPedidos = pedidos.Select(p => p.IdPedido).ToList();
            ViewData["PRODUCTOS"] = await this.service.GetProductosPedidoViewAsync(idPedidos);
            return PartialView("_PedidosRestaurante", pedidos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _PedidosRestaurante(int idpedido, int estado)
        {
            await this.service.UpdateEstadoPedidoAsync(idpedido, estado);
            return RedirectToAction("Index", new { nomvista = "_PedidosRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _EditRestaurante()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Restaurante rest = await this.service.GetRestauranteFromLoggedUserAsync();
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasRestaurantesAsync();
            return PartialView("_EditRestaurante", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _EditRestaurante(Restaurante rest, IFormFile fileimagen)
        {
            await this.service.EditRestauranteAsync(rest, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_EditRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _ProductosRestaurante()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Producto> productos;
            Restaurante rest = await this.service.GetRestauranteFromLoggedUserAsync();
            productos = await this.service.GetProductosRestauranteAsync(rest.IdRestaurante);
            ViewData["IDRESTAURANTE"] = rest.IdRestaurante;
            productos.ForEach(p => p.Imagen = UrlBlobProductos + p.Imagen);
            return PartialView("_ProductosRestaurante", productos);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _CreateProducto()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Restaurante rest = await this.service.GetRestauranteFromLoggedUserAsync();
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasProductosAsync(rest.IdRestaurante);
            return PartialView("_CreateProducto", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _CreateProducto(Producto prod, int[] categproducto, IFormFile fileimagen)
        {
            await this.service.CreateProductoAsync(prod, categproducto, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _DetailsProducto(int idprod)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.service.FindProductoAsync(idprod);
            prod.Imagen = UrlBlobProductos + prod.Imagen;
            return PartialView("_DetailsProducto", prod);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _EditProducto(int idprod)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.service.FindProductoAsync(idprod);
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasProductosAsync(prod.IdRestaurante);
            return PartialView("_EditProducto", prod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _EditProducto(Producto prod, int[] categproducto, IFormFile fileimagen)
        {
            await this.service.EditProductoAsync(prod, categproducto, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.service.DeleteProductoAsync(id);
            return RedirectToAction("Index", new { nomvista = "_ProductosRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _CategoriasRestaurante()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Restaurante rest = await this.service.GetRestauranteFromLoggedUserAsync();
            List<CategoriaProducto> categorias = await this.service.GetCategoriasProductosAsync(rest.IdRestaurante);
            ViewData["IDRESTAURANTE"] = rest.IdRestaurante;
            ViewData["BLOBURL"] = UrlBlobProductos;
            return PartialView("_CategoriasRestaurante", categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _CategoriasRestaurante(string categoria)
        {
            Restaurante rest = await this.service.GetRestauranteFromLoggedUserAsync();
            CategoriaProducto categProducto = await this.service.CreateCategoriaProductoAsync(rest.IdRestaurante, categoria);
            return RedirectToAction("Index", new { nomvista = "_CategoriasRestaurante" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> DeleteCategoriaProducto(int idcategoria)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "3")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.service.DeleteCategoriaProductoAsync(idcategoria);
            return RedirectToAction("Index", new { nomvista = "_CategoriasRestaurante" });
        }
    }
}
