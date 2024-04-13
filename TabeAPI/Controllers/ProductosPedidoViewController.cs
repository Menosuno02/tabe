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


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ProductoPedidoView>>> GetProductosPedidoView([FromQuery] List<int> idpedido)
        {
            return await this.repo.GetProductosPedidoViewAsync(idpedido);
        }
    }
}
