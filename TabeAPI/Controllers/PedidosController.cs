using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET;
using ProyectoASPNET.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public PedidosController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpPost]
        [Route("{idusuario}/{idrestaurante}")]
        public async Task<ActionResult<Pedido>> CreatePedido
            (int idusuario, int idrestaurante, List<ProductoCesta> cesta)
        {
            return await this.repo.CreatePedidoAsync(idusuario, idrestaurante, cesta);
        }

        [HttpGet]
        [Route("[action]/{idusuario}")]
        public async Task<ActionResult<List<Pedido>>>
            GetPedidosUsuario(int idusuario)
        {
            return await this.repo.GetPedidosUsuarioAsync(idusuario);
        }

        [HttpGet]
        [Route("[action]/{idusurestaurante}")]
        public async Task<ActionResult<List<Pedido>>>
            GetPedidosRestaurante(int idusurestaurante)
        {
            return await this.repo.GetPedidosRestauranteAsync(idusurestaurante);
        }
    }
}
