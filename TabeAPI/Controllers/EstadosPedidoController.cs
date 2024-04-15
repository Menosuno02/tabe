using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        // GET: api/EstadosPedido
        /// <summary>
        /// Devuelve todos los tipos de estado para un pedido
        /// </summary>
        /// <remarks>
        /// Permite obtener los estados para un pedido
        /// </remarks>
        /// <response code="200">Devuelve el conjunto de estados para los pedidos</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<EstadoPedido>>> GetEstadosPedido()
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
            {
                return await this.repo.GetEstadoPedidosAsync();
            }
            return Unauthorized();
        }

        // PUT: api/EstadosPedido/UpdateEstadoPedido
        /// <summary>
        /// Modifica el estado de un pedido en concreto
        /// </summary>
        /// <remarks>
        /// Permite modificar el estado de un pedido al recibir su ID y el ID del nuevo estado al que cambiar
        /// </remarks>
        /// <param name="model">Modelo con el ID del pedido y el ID del estado</param>
        /// <response code="200">Estado del pedido modificado con éxito</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin</response>
        [HttpPut]
        [Route("UpdateEstadoPedido")]
        [Authorize]
        public async Task<ActionResult> UpdateEstadoPedido(EstadoPedidoAPIModel model)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
            {
                await this.repo.UpdateEstadoPedidoAsync(model.IdPedido, model.Estado);
                return Ok();
            }
            return Unauthorized();
        }
    }
}
