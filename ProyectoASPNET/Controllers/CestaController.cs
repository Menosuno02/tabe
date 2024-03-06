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

        public async Task<IActionResult> Index()
        {
            CestaView cestaView = await helperCesta.GetDatosCesta();
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
            CestaView cestaView = await helperCesta.GetDatosCesta();
            return View(cestaView);
        }

        public IActionResult UpdateCesta(int idproducto, int cantidad)
        {
            this.helperCesta.UpdateProductoCesta(idproducto, cantidad);
            return RedirectToAction("Index");
        }
    }
}
