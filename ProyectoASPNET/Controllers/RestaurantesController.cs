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
            List<Producto> productos = await this.repo.GetProductosRestaurante(id);
            ViewData["RESTAURANTE"] = await this.repo.FindRestaurante(id);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriaProductos();
            return View(productos);
        }

        [HttpPost]
        public async Task<IActionResult> Productos(int id, int categoria)
        {
            List<Producto> productos = new List<Producto>();
            if (categoria == 0)
            {
                productos = await this.repo.GetProductosRestaurante(id);
            }
            else
            {
                productos = await this.repo.GetProductoCategorias(id, categoria);
            }
            ViewData["RESTAURANTE"] = await this.repo.FindRestaurante(id);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriaProductos();
            ViewData["SELECTED"] = categoria;
            return View(productos);
        }
    }
}
