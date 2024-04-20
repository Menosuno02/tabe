using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoASPNET;
using TabeNuget;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<EstadoPedido>>> GetEstadosPedido()
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
                return await this.repo.GetEstadoPedidosAsync();
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
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin, o es un Restaurante intentando editar el estado de un pedido a otro restaurante</response>
        /// <response code="404">Pedido no encontrado</response>
        [HttpPut]
        [Route("UpdateEstadoPedido")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateEstadoPedido(EstadoPedidoAPIModel model)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
            {
                Pedido pedido = await this.repo.FindPedidoAsync(model.IdPedido);
                if (pedido == null)
                    return NotFound();
                if (usuario.TipoUsuario == 3)
                {
                    Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(usuario.IdUsuario);
                    if (rest.IdRestaurante != pedido.IdRestaurante)
                        return Unauthorized();
                }
                await this.repo.UpdateEstadoPedidoAsync(model.IdPedido, model.Estado);
                return Ok();
            }
            return Unauthorized();
        }
    }
}
