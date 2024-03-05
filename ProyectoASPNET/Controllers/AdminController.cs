using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class AdminController : Controller
    {
        private RepositoryRestaurantes repo;

        public AdminController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Usuarios()
        {
            List<Usuario> usuarios = await this.repo.GetUsuariosAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Restaurantes()
        {
            List<RestauranteView> restaurantes = await this.repo.GetRestaurantesAsync();
            return View(restaurantes);
        }

        public async Task<IActionResult> Productos(int id)
        {
            List<Producto> productos = await this.repo.GetProductosRestauranteAsync(id);
            return View(productos);
        }
    }
}
