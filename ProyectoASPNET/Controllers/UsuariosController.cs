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

        public async Task<IActionResult> Carro()
        {
            decimal total = 0;
            List<ProductoCesta> cesta = new List<ProductoCesta>();
            foreach (ProductoCesta prodCesta in helperCesta.GetCesta())
            {
                total += prodCesta.Precio;
            }
            ViewData["TOTAL"] = total;
            return View(cesta);
        }
    }
}
