using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class RestaurantesController : Controller
    {
        private RepositoryRestaurantes repo;

        public RestaurantesController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<RestauranteView> restaurantes = await this.repo.GetRestaurantes();
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantes();
            return View(restaurantes);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string categoria, int rating)
        {
            List<RestauranteView> restaurantes = await this.repo.FilterRestaurantes(categoria, rating);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantes();
            return View(restaurantes);
        }

        public async Task<IActionResult> Productos(int id)
        {
            ProductosActionModel model = new ProductosActionModel
            {
                Productos = await this.repo.GetProductosRestaurante(id),
                Restaurante = await this.repo.FindRestaurante(id),
                CategoriasProductos = await this.repo.GetCategoriaProductos(),
                SelectedCategoria = 0
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Productos(int id, int categoria)
        {
            ProductosActionModel model = new ProductosActionModel
            {
                Restaurante = await this.repo.FindRestaurante(id),
                CategoriasProductos = await this.repo.GetCategoriaProductos(),
                SelectedCategoria = categoria
            };
            if (categoria == 0)
                model.Productos = await this.repo.GetProductosRestaurante(id);
            else
                model.Productos = await this.repo.GetProductoCategorias(id, categoria);
            return View(model);
        }
    }
}
