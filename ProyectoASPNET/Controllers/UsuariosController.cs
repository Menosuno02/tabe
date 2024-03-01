using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class UsuariosController : Controller
    {
        private RepositoryRestaurantes repo;
        private HelperCesta helperCesta;

        public UsuariosController
            (RepositoryRestaurantes repo,
            HelperCesta helperCesta)
        {
            this.repo = repo;
            this.helperCesta = helperCesta;
        }

        public async Task<IActionResult> Cesta()
        {
            decimal total = 0;
            List<ProductoCestaView> cestaView = new List<ProductoCestaView>();
            List<ProductoCesta> cesta = helperCesta.GetCesta();
            if (cesta != null)
            {
                foreach (ProductoCesta prodCesta in cesta)
                {
                    Producto prod = await this.repo.FindProducto(prodCesta.IdProducto);
                    cestaView.Add(new ProductoCestaView
                    {
                        IdProducto = prodCesta.IdProducto,
                        Nombre = prod.Nombre,
                        Precio = prod.Precio * prodCesta.Cantidad,
                        Cantidad = prodCesta.Cantidad,
                        Imagen = prod.Imagen
                    });
                    total += prod.Precio * prodCesta.Cantidad;
                }
            }
            ViewData["TOTAL"] = total;
            return View(cestaView);
        }

        [HttpPost]
        public async Task<IActionResult> Cesta(string form, int idproducto)
        {
            if (form == "borrar")
            {
                this.helperCesta.DeleteProductoCesta(idproducto);
            }
            decimal total = 0;
            List<ProductoCestaView> cestaView = new List<ProductoCestaView>();
            List<ProductoCesta> cesta = helperCesta.GetCesta();
            if (cesta != null)
            {
                foreach (ProductoCesta prodCesta in cesta)
                {
                    Producto prod = await this.repo.FindProducto(prodCesta.IdProducto);
                    cestaView.Add(new ProductoCestaView
                    {
                        IdProducto = prodCesta.IdProducto,
                        Nombre = prod.Nombre,
                        Precio = prod.Precio * prodCesta.Cantidad,
                        Cantidad = prodCesta.Cantidad,
                        Imagen = prod.Imagen
                    });
                    total += prod.Precio * prodCesta.Cantidad;
                }
            }
            ViewData["TOTAL"] = total;
            return View(cestaView);
        }
    }
}
