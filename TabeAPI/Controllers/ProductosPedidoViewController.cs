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

        
        [HttpGet("{idpedidos}")]
        public async Task<ActionResult<List<ProductoPedidoView>>> GetProductosPedidoView(string idpedidos)
        {
            return await this.repo.GetProductosPedidoViewAsync(idpedidos.Split(",").Select(int.Parse).ToList());
        }
    }
}
