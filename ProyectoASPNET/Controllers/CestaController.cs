using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class CestaController : Controller
    {
        private RepositoryRestaurantes repo;
        private HelperCesta helperCesta;

        public CestaController
            (RepositoryRestaurantes repo,
            HelperCesta helperCesta)
        {
            this.repo = repo;
            this.helperCesta = helperCesta;
        }

        private async Task<CestaView> GetDatosCesta()
        {
            decimal total = 0;
            List<ProductoCestaView> cestaView = new List<ProductoCestaView>();
            List<ProductoCesta> cesta = helperCesta.GetCesta();
            if (cesta != null)
            {
                foreach (ProductoCesta prodCesta in cesta)
                {
                    Producto prod = await this.repo.FindProductoAsync(prodCesta.IdProducto);
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
            int id = HttpContext.Session.GetObject<int>("USER");
            Usuario usuario = await this.repo.FindUsuarioAsync(id);
            return new CestaView
            {
                Cesta = cestaView,
                Total = total,
                Nombre = usuario.Nombre + " " + usuario.Apellidos,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono
            };
        }

        public async Task<IActionResult> Index()
        {
            CestaView cestaView = await GetDatosCesta();
            return View(cestaView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string form, int? idproducto)
        {
            if (form == "borrar" && idproducto != null)
            {
                this.helperCesta.DeleteProductoCesta(idproducto.Value);
            }
            else if (form == "pagar")
            {
                await this.helperCesta.CreatePedido();
                return RedirectToAction("Index", "Restaurantes");
            }
            CestaView cestaView = await GetDatosCesta();
            return View(cestaView);
        }

        public IActionResult UpdateCesta(int idproducto, int cantidad)
        {
            this.helperCesta.UpdateProductoCesta(idproducto, cantidad);
            return RedirectToAction("Index");
        }
    }
}
