using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Filters;
using TabeNuget;
using ProyectoASPNET.Services;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class PanelAdminController : Controller
    {
        private IServiceRestaurantes service;
        private string UrlBlobProductos;
        private string UrlBlobRestaurantes;


        public PanelAdminController(IServiceRestaurantes service, IConfiguration configuration)
        {
            this.service = service;
            UrlBlobProductos = configuration.GetValue<string>("BlobUrls:UrlProductos");
            UrlBlobRestaurantes = configuration.GetValue<string>("BlobUrls:UrlRestaurantes");
        }

        [AuthorizeUser]
        public IActionResult Index(string? nomvista, int? idrest)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            if (nomvista != null)
            {
                ViewData["NOMVISTA"] = nomvista;
            }
            if (idrest != null)
            {
                ViewData["IDREST"] = idrest;
            }
            return View();
        }

        #region RESTAURANTES
        [AuthorizeUser]
        public async Task<IActionResult> _Restaurantes()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<RestauranteView> restaurantes = await this.service.GetRestaurantesViewAsync("");
            return PartialView("_Restaurantes", restaurantes);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _CreateRestaurante()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<CategoriaRestaurante> categorias = await this.service.GetCategoriasRestaurantesAsync();
            return PartialView("_CreateRestaurante", categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _CreateRestaurante(Restaurante rest, string contrasenya, IFormFile fileimagen)
        {
            await this.service.CreateRestauranteAsync(rest, contrasenya, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_Restaurantes" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _DetailsRestaurante(int idrest)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            RestauranteView rest = await this.service.FindRestauranteViewAsync(idrest);
            rest.Imagen = UrlBlobRestaurantes + rest.Imagen;
            return PartialView("_DetailsRestaurante", rest);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _EditRestaurante(int idrest)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Restaurante rest = await this.service.FindRestauranteAsync(idrest);
            ViewData["CATEGORIAS"] = await this.service.GetCategoriasRestaurantesAsync();
            return PartialView("_EditRestaurante", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _EditRestaurante(Restaurante rest, IFormFile fileimagen)
        {
            await this.service.EditRestauranteAsync(rest, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_Restaurantes" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> DeleteRestaurante(int id)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.service.DeleteRestauranteAsync(id);
            return RedirectToAction("Index", new { nomvista = "_Restaurantes" });
        }
        #endregion

        #region USUARIOS
        [AuthorizeUser]
        public async Task<IActionResult> _Usuarios()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Usuario> usuarios = await this.service.GetUsuariosAsync();
            return PartialView("_Usuarios", usuarios);
        }
        #endregion

        #region PRODUCTOS
        [AuthorizeUser]
        public async Task<IActionResult> _Productos(int? idrest)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Producto> productos;
            if (idrest != null)
            {
                productos = await this.service.GetProductosRestauranteAsync(idrest.Value);
                ViewData["IDRESTAURANTE"] = idrest.Value;
            }
            else
            {
                productos = await this.service.GetProductosAsync();
            }
            productos.ForEach(p => p.Imagen = UrlBlobProductos + p.Imagen);
            return PartialView("_Productos", productos);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _CreateProducto(int? idrestaurante)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            if (idrestaurante != null)
            {
                ViewData["RESTAURANTE"] = await this.service.FindRestauranteAsync(idrestaurante.Value);
                ViewData["CATEGORIAS"] = await this.service.GetCategoriasProductosAsync(idrestaurante.Value);
                return PartialView("_CreateProducto");
            }
            List<Restaurante> restaurantes = await this.service.GetRestaurantesAsync();
            return PartialView("_CreateProducto", restaurantes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _CreateProducto(Producto prod, int[] categproducto, IFormFile fileimagen)
        {
            await this.service.CreateProductoAsync(prod, categproducto, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_Productos", idrest = prod.IdRestaurante });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _DetailsProducto(int idprod)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
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
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
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
            return RedirectToAction("Index", new { nomvista = "_Productos", idrest = prod.IdRestaurante });
        }

        [AuthorizeUser]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "2")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.service.FindProductoAsync(id);
            await this.service.DeleteProductoAsync(id);
            return RedirectToAction("Index", new { nomvista = "_Productos", idrest = prod.IdRestaurante });
        }

        public async Task<IActionResult> _CategoriasProductoRestaurante(int idrestaurante)
        {
            List<CategoriaProducto> categorias = await this.service.GetCategoriasProductosAsync(idrestaurante);
            return PartialView("_CategoriasProductoRestaurante", categorias);
        }
        #endregion
    }
}
