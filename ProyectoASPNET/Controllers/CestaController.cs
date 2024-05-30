using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Filters;
using ProyectoASPNET.Helpers;
using TabeNuget;
using ProyectoASPNET.Services;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class CestaController : Controller
    {
        private IServiceRestaurantes service;
        private ServiceCacheRedis serviceRedis;
        private HelperMails helperMails;
        private ServiceLogicApps serviceLogicApps;
        public CestaController

            (IServiceRestaurantes service,
            ServiceCacheRedis serviceRedis,
            HelperMails helperMails,
            ServiceLogicApps serviceLogicApps)
        {
            this.service = service;
            this.serviceRedis = serviceRedis;
            this.helperMails = helperMails;
            this.serviceLogicApps = serviceLogicApps;
        }

        [AuthorizeUser]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            CestaView cestaView = await this.serviceRedis.GetDatosCesta();
            return View(cestaView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> Index(string form, int? idproducto)
        {
            if (form == "borrar" && idproducto != null)
            {
                await this.serviceRedis.DeleteProductoCesta(idproducto.Value);
            }
            else if (form == "pagar")
            {
                Pedido pedido = await this.serviceRedis.CreatePedido();
                List<ProductoPedidoView> productosPedido = await this.service.GetProductosPedidoViewAsync(new List<int> { pedido.IdPedido });
                string mensaje = "<h1 style=\"color: #6D28D9; margin-bottom:0px;\">Pedido realizado con éxito</h3>";

                mensaje += $"<div style=\"padding: 1.25rem; border-width: 1px; border-bottom-width: 0; border-color: #E5E7EB; \">" +
                    $"<p style=\"margin-bottom: 0.5rem; color: #4B5563; \"><span style=\"font-size: 1.5rem;line-height: 2rem; color: #374151; \">" +
                    $"<span style=\"font-weight: 700; \">{productosPedido.FirstOrDefault().Restaurante}</span>" +
                    $"&nbsp;&nbsp;•&nbsp;&nbsp;&nbsp;Fecha: {pedido.FechaPedido}" +
                    $"</span>" +
                    $"<div style=\"overflow-x: auto; position: relative; \">" +
                    $"<table style=\"width: 100%; font-size: 0.875rem;line-height: 1.25rem; text-align: left; color: #6B7280; \">" +
                    $"<thead style=\"font-size: 0.75rem;line-height: 1rem; color: #111827; text-transform: uppercase; background-color: #F3F4F6; \">" +
                    $"<tr>" +
                    $"<th scope=\"col\" style=\"padding-top: 0.75rem;padding-bottom: 0.75rem; padding-left: 1.5rem;padding-right: 1.5rem; \">Producto</th>" +
                    $"<th scope=\"col\" style=\"padding-top: 0.75rem;padding-bottom: 0.75rem; padding-left: 1.5rem;padding-right: 1.5rem; \">Cantidad</th>" +
                    $"<th scope=\"col\" style=\"padding-top: 0.75rem;padding-bottom: 0.75rem; padding-left: 1.5rem;padding-right: 1.5rem; \">Total</th>" +
                    $"</tr>" +
                    $"</thead>" +
                    $"<tbody>";

                foreach (ProductoPedidoView producto in productosPedido)
                {
                    decimal total = producto.Cantidad * producto.Precio;
                    mensaje += $"<tr style=\"background-color: #ffffff; \">" +
                        $"<th scope=\"row\" style=\"padding-top: 1rem;padding-bottom: 1rem; padding-left: 1.5rem;padding-right: 1.5rem; font-weight: 500; color: #111827; white-space: nowrap; \">{producto.Producto}</th>" +
                        $"<td style=\"padding-top: 1rem;padding-bottom: 1rem; padding-left: 1.5rem;padding-right: 1.5rem; \">{producto.Cantidad}</td>" +
                        $"<td style=\"padding-top: 1rem;padding-bottom: 1rem; padding-left: 1.5rem;padding-right: 1.5rem; \">{total}€</td>" +
                        $"</tr>";
                }
                mensaje += $"<tr style=\"background-color: #ffffff; \">" +
                    $"<th scope=\"row\" style=\"padding-top: 1rem;padding-bottom: 1rem; padding-left: 1.5rem;padding-right: 1.5rem; font-weight: 500; color: #111827; white-space: nowrap; \"></th>" +
                    $"<td style=\"padding-top: 1rem;padding-bottom: 1rem; padding-left: 1.5rem;padding-right: 1.5rem; \"></td>" +
                    $"<td style=\"padding-top: 1rem;padding-bottom: 1rem; padding-left: 1.5rem;padding-right: 1.5rem; font-weight: 700; color: #374151; \">{productosPedido.Sum(pp => pp.Cantidad * pp.Precio)}€</td>" +
                    $"</tr>" +
                    $"</tbody>" +
                    $"</table>" +
                    $"</div>" +
                    $"</p>" +
                    $"</div>";
                // await helperMails.SendMailAsync(HttpContext.User.Identity.Name, "Pedido realizado", mensaje);
                await serviceLogicApps.SendMailAsync(HttpContext.User.Identity.Name, "Pedido realizado", mensaje);
                return RedirectToAction("Index", "Restaurantes");
            }
            CestaView cestaView = await serviceRedis.GetDatosCesta();
            return View(cestaView);
        }

        [AuthorizeUser]
        public async Task<IActionResult> UpdateCesta(int idproducto, int cantidad)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.serviceRedis.UpdateProductoCesta(idproducto, cantidad);
            return RedirectToAction("Index");
        }
    }
}
