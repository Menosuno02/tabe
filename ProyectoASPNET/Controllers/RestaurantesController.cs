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

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 1)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            ViewData["CATEGORIAS"] =
                await this.repo.GetCategoriasRestaurantesAsync();
            return View();
        }

        public async Task<IActionResult> _ListRestaurantes(string? categoria, string searchquery = "")
        {
            List<RestauranteView> restaurantes;
            if (categoria != null)
            {
                restaurantes = await this.repo.FilterRestaurantesViewAsync(categoria, searchquery);
            }
            else
            {
                restaurantes = await this.repo.GetRestaurantesViewAsync(searchquery);
            }
            int idusuario = HttpContext.Session.GetObject<int>("USER");
            Usuario usu = await this.repo.FindUsuarioAsync(idusuario);
            string direccionUsu = usu.Direccion;

            /*
            foreach (RestauranteView rest in restaurantes)
            {
                rest.InfoEntrega =
                    await this.helperDistanceMatrix
                    .GetDistanceMatrixInfoAsync(rest.Direccion, direccionUsu);
            }
            */

            var tasks = restaurantes
                .Select(async r => r.InfoEntrega = await this.helperDistanceMatrix
                    .GetDistanceMatrixInfoAsync(r.Direccion, direccionUsu));
            await Task.WhenAll(tasks);
            return PartialView("_ListRestaurantes", restaurantes);
        }

        public async Task<IActionResult> Productos(int idrestaurante)
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 1)
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            ProductosActionModel model = new ProductosActionModel
            {
                Restaurante = await this.repo.FindRestauranteViewAsync(idrestaurante),
                CategoriasProductos = await this.repo.GetCategoriasProductosAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                CategoriasProductos = await this.repo.GetCategoriasProductosAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> _ListProductos(int idrestaurante, int categoria)
        {
            List<Producto> productos;
            if (categoria == 0)
                productos = await this.repo.GetProductosRestauranteAsync(idrestaurante);
            else
                productos = await this.repo.GetProductosByCategoriaAsync(idrestaurante, categoria);
            return PartialView("_ListProductos", productos);
        }

        public async Task<IActionResult> UpdateValoracionRestaurante
            (int idrestaurante, int idusuario, int valoracion)
        {
            if (HttpContext.Session.GetString("USER") == null ||
                HttpContext.Session.GetObject<int>("TIPOUSER") != 1)
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
