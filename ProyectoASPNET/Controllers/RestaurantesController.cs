using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Models;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Helpers;
using Microsoft.AspNetCore.Http;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string categoria, int rating)
        {
            List<RestauranteView> restaurantes = await this.repo.FilterRestaurantesAsync(categoria, rating);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantesAsync();
            return View(restaurantes);
        }

        public async Task<IActionResult> Productos(int idrestaurante)
        {
            ProductosActionModel model = new ProductosActionModel
            {
                Productos = await this.repo.GetProductosRestauranteAsync(idrestaurante),
                Restaurante = await this.repo.FindRestauranteAsync(idrestaurante),
                CategoriasProductos = await this.repo.GetCategoriaProductosAsync(),
                SelectedCategoria = 0
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Productos
            (string form, int idrestaurante, int categoria,
            int cantidad, int idproducto)
        {
            if (form == "cesta")
            {
                int restauranteSession = HttpContext.Session.GetObject<int>("RESTAURANTE");
                // Si id (idrestaurante) no es igual al idrestuarante en el Session, nos salta error
                // Solo podemos tener productos en nuestra cesta de un solo restaurante
                if (restauranteSession == 0
                    || restauranteSession == idrestaurante)
                {
                    await helperCesta.UpdateCesta(new ProductoCesta
                    {
                        IdProducto = idproducto,
                        Cantidad = cantidad
                    });
                }
                else
                {
                    ViewData["MENSAJE"] = "Error";
                }
            }
            ProductosActionModel model = new ProductosActionModel
            {
                Restaurante = await this.repo.FindRestauranteAsync(idrestaurante),
                CategoriasProductos = await this.repo.GetCategoriaProductosAsync(),
                SelectedCategoria = categoria
            };
            if (categoria == 0)
                model.Productos =
                    await this.repo.GetProductosRestauranteAsync(idrestaurante);
            else
                model.Productos =
                    await this.repo.GetProductosByCategoriaAsync(idrestaurante, categoria);
            return View(model);
        }
    }
}
