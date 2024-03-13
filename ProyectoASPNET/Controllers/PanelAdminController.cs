using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Filters;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class PanelAdminController : Controller
    {
        private RepositoryRestaurantes repo;

        public PanelAdminController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [AuthorizeUser]
        public IActionResult Index(string? nomvista, int? idrest)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
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
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<RestauranteView> restaurantes = await this.repo.GetRestaurantesViewAsync("");
            return PartialView("_Restaurantes", restaurantes);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _CreateRestaurante()
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<CategoriaRestaurante> categorias = await this.repo.GetCategoriasRestaurantesAsync();
            return PartialView("_CreateRestaurante", categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _CreateRestaurante(Restaurante rest, string contrasenya, IFormFile fileimagen)
        {
            await this.repo.CreateRestauranteAsync(rest, contrasenya, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_Restaurantes" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _DetailsRestaurante(int idrest)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            RestauranteView rest = await this.repo.FindRestauranteViewAsync(idrest);
            return PartialView("_DetailsRestaurante", rest);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _EditRestaurante(int idrest)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Restaurante rest = await this.repo.FindRestauranteAsync(idrest);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantesAsync();
            return PartialView("_EditRestaurante", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _EditRestaurante(Restaurante rest, IFormFile fileimagen)
        {
            await this.repo.EditRestauranteAsync(rest, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_Restaurantes" });
        }

        [AuthorizeUser]
        public async Task<IActionResult> DeleteRestaurante(int id)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.repo.DeleteRestauranteAsync(id);
            return RedirectToAction("Index", new { nomvista = "_Restaurantes" });
        }
        #endregion

        #region USUARIOS
        [AuthorizeUser]
        public async Task<IActionResult> _Usuarios()
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Usuario> usuarios = await this.repo.GetUsuariosAsync();
            return PartialView("_Usuarios", usuarios);
        }
        #endregion

        #region PRODUCTOS
        [AuthorizeUser]
        public async Task<IActionResult> _Productos(int? idrest)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<Producto> productos;
            if (idrest != null)
            {
                productos = await this.repo.GetProductosRestauranteAsync(idrest.Value);
                ViewData["IDRESTAURANTE"] = idrest.Value;
            }
            else
            {
                productos = await this.repo.GetProductosAsync();
            }
            return PartialView("_Productos", productos);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _CreateProducto(int? idrestaurante)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            if (idrestaurante != null)
            {
                ViewData["RESTAURANTE"] = await this.repo.FindRestauranteAsync(idrestaurante.Value);
                ViewData["CATEGORIAS"] = await this.repo.GetCategoriasProductosAsync(idrestaurante.Value);
                return PartialView("_CreateProducto");
            }
            List<Restaurante> restaurantes = await this.repo.GetRestaurantesAsync();
            return PartialView("_CreateProducto", restaurantes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> _CreateProducto(Producto prod, int[] categproducto, IFormFile fileimagen)
        {
            await this.repo.CreateProductoAsync(prod, categproducto, fileimagen);
            return RedirectToAction("Index", new { nomvista = "_Productos", idrest = prod.IdRestaurante });
        }

        [AuthorizeUser]
        public async Task<IActionResult> _DetailsProducto(int idprod)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.repo.FindProductoAsync(idprod);
            return PartialView("_DetailsProducto", prod);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _EditProducto(int idprod)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
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
            return RedirectToAction("Index", new { nomvista = "_Productos", idrest = prod.IdRestaurante });
        }

        [AuthorizeUser]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (HttpContext.Session.GetObject<int>("TIPOUSER") != 2)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            Producto prod = await this.repo.FindProductoAsync(id);
            await this.repo.DeleteProductoAsync(id);
            return RedirectToAction("Index", new { nomvista = "_Productos", idrest = prod.IdRestaurante });
        }

        public async Task<IActionResult> _CategoriasProductoRestaurante(int idrestaurante)
        {
            List<CategoriaProducto> categorias = await this.repo.GetCategoriasProductosAsync(idrestaurante);
            return PartialView("_CategoriasProductoRestaurante", categorias);
        }
        #endregion
    }
}
