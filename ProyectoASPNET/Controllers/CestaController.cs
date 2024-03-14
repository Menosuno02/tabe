using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Filters;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Models;
using System.Security.Claims;

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

        [AuthorizeUser]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            CestaView cestaView = await helperCesta.GetDatosCesta();
            return View(cestaView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
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

        [AuthorizeUser]
        public IActionResult UpdateCesta(int idproducto, int cantidad)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            this.helperCesta.UpdateProductoCesta(idproducto, cantidad);
            return RedirectToAction("Index");
        }
    }
}
