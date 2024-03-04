using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Models;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Helpers;

namespace ProyectoASPNET.Controllers
{
    public class RestaurantesController : Controller
    {
        private RepositoryRestaurantes repo;
        private HelperCesta helperCesta;

        public RestaurantesController
            (RepositoryRestaurantes repo,
            HelperCesta helperCesta)
        {
            this.repo = repo;
            this.helperCesta = helperCesta;
        }

        public async Task<IActionResult> Index()
        {
            List<RestauranteView> restaurantes = await this.repo.GetRestaurantesAsync();
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantesAsync();
            return View(restaurantes);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string categoria, int rating)
        {
            List<RestauranteView> restaurantes = await this.repo.FilterRestaurantesAsync(categoria, rating);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantesAsync();
            return View(restaurantes);
        }

        public async Task<IActionResult> Productos(int id)
        {
            ProductosActionModel model = new ProductosActionModel
            {
                Productos = await this.repo.GetProductosRestauranteAsync(id),
                Restaurante = await this.repo.FindRestauranteAsync(id),
                CategoriasProductos = await this.repo.GetCategoriaProductosAsync(),
                SelectedCategoria = 0
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Productos
            (string form, int id, int categoria,
            int cantidad, int idProducto)
        {
            if (form == "cesta")
            {
                helperCesta.UpdateCesta(new ProductoCesta
                {
                    IdProducto = idProducto,
                    Cantidad = cantidad
                });
                ViewData["CESTA"] = HttpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA");
            }
            ProductosActionModel model = new ProductosActionModel
            {
                Restaurante = await this.repo.FindRestauranteAsync(id),
                CategoriasProductos = await this.repo.GetCategoriaProductosAsync(),
                SelectedCategoria = categoria
            };
            if (categoria == 0)
                model.Productos = await this.repo.GetProductosRestauranteAsync(id);
            else
                model.Productos = await this.repo.GetProductoCategoriasAsync(id, categoria);
            return View(model);
        }
    }
}
