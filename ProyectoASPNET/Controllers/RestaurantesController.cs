﻿using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Models;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Filters;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class RestaurantesController : Controller
    {
        private RepositoryRestaurantes repo;
        private HelperCesta helperCesta;
        private HelperGoogleApiDirections helperDistanceMatrix;

        public RestaurantesController
            (RepositoryRestaurantes repo,
            HelperCesta helperCesta,
            HelperGoogleApiDirections helperDistanceMatrix)
        {
            this.repo = repo;
            this.helperCesta = helperCesta;
            this.helperDistanceMatrix = helperDistanceMatrix;
        }

        [AuthorizeUser]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            ViewData["CATEGORIAS"] =
                await this.repo.GetCategoriasRestaurantesAsync();
            return View();
        }

        [AuthorizeUser]
        public async Task<IActionResult> _ListRestaurantes
            (string? categoria, string searchquery = "", string orden = "valoracion", int posicion = 1)
        {
            PaginationRestaurantesView model = new PaginationRestaurantesView();
            if (categoria != null)
                model = await this.repo.FilterPaginationRestaurantesViewAsync(categoria, searchquery, posicion);
            else
                model = await this.repo.GetPaginationRestaurantesViewAsync(searchquery, posicion);
            List<RestauranteView> restaurantes = model.Restaurantes;
            ViewData["POSICION"] = posicion;
            ViewData["NUMREGISTROS"] = model.NumRegistros;
            int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Usuario usu = await this.repo.FindUsuarioAsync(idusuario);
            string direccionUsu = usu.Direccion;
            var tasks = restaurantes
                .Select(async r => r.InfoEntrega = await this.helperDistanceMatrix
                    .GetDistanceMatrixInfoAsync(r.Direccion, direccionUsu));
            await Task.WhenAll(tasks);
            if (orden != "valoracion")
                restaurantes = restaurantes.OrderBy(r => r.InfoEntrega.TiempoEstimado).ToList();
            return PartialView("_ListRestaurantes", restaurantes);
        }

        [AuthorizeUser]
        public async Task<IActionResult> Productos(int idrestaurante)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            ProductosActionModel model = new ProductosActionModel
            {
                Restaurante = await this.repo.FindRestauranteViewAsync(idrestaurante),
                CategoriasProductos = await this.repo.GetCategoriasProductosAsync(idrestaurante)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> Productos
            (string form, int idrestaurante, int cantidad, int idproducto)
        {
            if (form == "cesta")
            {
                int restauranteSession = HttpContext.Session.GetObject<int>("RESTAURANTE");
                // Si id (idrestaurante) no es igual al idrestuarante en el Session, nos salta error
                // Solo podemos tener productos en nuestra cesta de un solo restaurante
                if (restauranteSession == 0
                    || (restauranteSession != 0 && restauranteSession == idrestaurante))
                {
                    await helperCesta.UpdateCesta(new ProductoCesta
                    {
                        IdProducto = idproducto,
                        Cantidad = cantidad
                    });
                }
                else ViewData["ERROR"] = true;
            }
            ProductosActionModel model = new ProductosActionModel
            {
                Restaurante = await this.repo.FindRestauranteViewAsync(idrestaurante),
                CategoriasProductos = await this.repo.GetCategoriasProductosAsync(idrestaurante)
            };
            return View(model);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _ListProductos(int idrestaurante, int categoria)
        {
            List<Producto> productos;
            if (categoria == 0)
                productos = await this.repo.GetProductosRestauranteAsync(idrestaurante);
            else
                productos = await this.repo.GetProductosByCategoriaAsync(idrestaurante, categoria);
            return PartialView("_ListProductos", productos);
        }

        [AuthorizeUser]
        public async Task<IActionResult> UpdateValoracionRestaurante
            (int idrestaurante, int idusuario, int valoracion)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.repo.UpdateValoracionRestauranteAsync(
                new ValoracionRestaurante
                {
                    IdRestaurante = idrestaurante,
                    IdUsuario = idusuario,
                    Valoracion = valoracion
                });
            return RedirectToAction("Productos", new { idrestaurante = idrestaurante });
        }
    }
}
