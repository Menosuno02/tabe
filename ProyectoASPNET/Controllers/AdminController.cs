using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Models;
using System.Collections.Generic;

namespace ProyectoASPNET.Controllers
{
    public class AdminController : Controller
    {
        private RepositoryRestaurantes repo;

        public AdminController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        #region RESTAURANTES
        public async Task<IActionResult> Restaurantes()
        {
            List<RestauranteView> restaurantes = await this.repo.GetRestaurantesViewAsync();
            return View(restaurantes);
        }

        public async Task<IActionResult> CreateRestaurante()
        {
            List<CategoriaRestaurante> categorias = await this.repo.GetCategoriasRestaurantesAsync();
            return View(categorias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRestaurante(Restaurante rest)
        {
            await this.repo.CreateRestauranteAsync(rest);
            return RedirectToAction("Restaurantes");
        }

        public async Task<IActionResult> _DetailsRestaurante(int idrest)
        {
            RestauranteView rest = await this.repo.FindRestauranteViewAsync(idrest);
            return PartialView("_DetailsRestaurante", rest);
        }

        public async Task<IActionResult> _EditRestaurante(int idrest)
        {
            Restaurante rest = await this.repo.FindRestauranteAsync(idrest);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasRestaurantesAsync();
            return PartialView("_EditRestaurante", rest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditRestaurante(Restaurante rest)
        {
            await this.repo.EditRestauranteAsync(rest);
            return RedirectToAction("Restaurantes");
        }

        public async Task<IActionResult> DeleteRestaurante(int id)
        {
            await this.repo.DeleteRestauranteAsync(id);
            return RedirectToAction("Restaurantes");
        }
        #endregion

        #region USUARIOS
        public async Task<IActionResult> Usuarios()
        {
            List<Usuario> usuarios = await this.repo.GetUsuariosAsync();
            return View(usuarios);
        }
        #endregion

        #region PRODUCTOS
        public async Task<IActionResult> Productos(int? id)
        {
            List<Producto> productos;
            if (id != null)
            {
                productos = await this.repo.GetProductosRestauranteAsync(id.Value);
                ViewData["IDRESTAURANTE"] = id.Value;
            }
            else
            {
                productos = await this.repo.GetProductosAsync();
            }
            return View(productos);
        }

        public async Task<IActionResult> CreateProducto(int? idrestaurante)
        {
            if (idrestaurante != null)
            {
                ViewData["RESTAURANTE"] = await this.repo.FindRestauranteAsync(idrestaurante.Value);
                return View();
            }
            List<Restaurante> restaurantes = await this.repo.GetRestaurantesAsync();
            return View(restaurantes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProducto(Producto prod)
        {
            await this.repo.CreateProductoAsync(prod);
            return RedirectToAction("Productos");
        }

        public async Task<IActionResult> _DetailsProducto(int idprod)
        {
            Producto prod = await this.repo.FindProductoAsync(idprod);
            return PartialView("_DetailsProducto", prod);
        }

        public async Task<IActionResult> _EditProducto(int idprod)
        {
            Producto prod = await this.repo.FindProductoAsync(idprod);
            ViewData["CATEGORIAS"] = await this.repo.GetCategoriasProductosAsync();
            return PartialView("_EditProducto", prod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditProducto(Producto prod, int[] categproducto)
        {
            await this.repo.EditProductoAsync(prod, categproducto);
            return RedirectToAction("Productos");
        }

        public async Task<IActionResult> DeleteProducto(int id)
        {
            await this.repo.DeleteProductoAsync(id);
            return RedirectToAction("Productos");
        }
        #endregion
    }
}
