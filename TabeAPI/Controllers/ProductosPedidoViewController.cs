using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET;
using ProyectoASPNET.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosPedidoViewController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public ProductosPedidoViewController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        // GET: api/ProductosPedidoView
        /// <summary>
        /// Devuelve los productos de una serie de pedidos
        /// </summary>
        /// <remarks>
        /// Permite obtener todos los productos de un conjunto de pedidos
        /// </remarks>
        /// <param name="idpedido">ID de los pedidos</param>
        /// <response code="200">Devuelve el conjunto de pedidos del usuario</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ProductoPedidoView>>> GetProductosPedidoView([FromQuery] List<int> idpedido)
        {
            return await this.repo.GetProductosPedidoViewAsync(idpedido);
        }
    }
}
