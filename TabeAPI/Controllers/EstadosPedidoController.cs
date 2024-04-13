using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET;
using ProyectoASPNET.Models;
using TabeAPI.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadosPedidoController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public EstadosPedidoController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<EstadoPedido>>> GetEstadosPedido()
        {
            return await this.repo.GetEstadoPedidosAsync();
        }

        [HttpPut]
        [Route("UpdateEstadoPedido")]
        [Authorize]
        public async Task<ActionResult>
            UpdateEstadoPedido(EstadoPedidoAPIModel model)
        {
            await this.repo.UpdateEstadoPedidoAsync(model.IdPedido, model.Estado);
            return Ok();
        }
    }
}
