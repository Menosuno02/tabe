using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        // GET: api/Pedidos/GetPedidosUsuario
        /// <summary>
        /// Devuelve los pedidos del usuario de tipo Usuario logeado
        /// </summary>
        /// <remarks>
        /// Permite obtener todos los pedidos de un usuario logeado (tipo Usuario) de la BBDD
        /// </remarks>
        /// <response code="200">Devuelve el conjunto de pedidos del usuario</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Usuario o Admin</response>
        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<List<Pedido>>> GetPedidosUsuario()
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 3)
                return await this.repo.GetPedidosUsuarioAsync(usuario.IdUsuario);
            return Unauthorized();
        }

        // GET: api/Pedidos/GetPedidosRestaurante
        /// <summary>
        /// Devuelve los pedidos del usuario de tipo Restaurante logeado
        /// </summary>
        /// <remarks>
        /// Permite obtener todos los pedidos de un usuario logeado (tipo Restaurante) de la BBDD
        /// </remarks>
        /// <response code="200">Devuelve el conjunto de pedidos del usuario</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin</response>
        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<List<Pedido>>> GetPedidosRestaurante()
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
                return await this.repo.GetPedidosRestauranteAsync(usuario);
            return Unauthorized();
        }

        // POST: api/Pedidos/{idrestaurante}
        /// <summary>
        /// Crea un nuevo pedido
        /// </summary>
        /// <remarks>
        /// Permite crear un nuevo pedido con el ID del restaurante y un conjunto de productos
        /// </remarks>
        /// <param name="idrestaurante">ID del pedido e ID del estado</param>
        /// <response code="200">Devuelve el nuevo pedido</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Usuario o Admin</response>
        [HttpPost]
        [Route("{idrestaurante}")]
        [Authorize]
        public async Task<ActionResult<Pedido>> CreatePedido(int idrestaurante, List<ProductoCesta> cesta)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 3)
                return await this.repo.CreatePedidoAsync(usuario.IdUsuario, idrestaurante, cesta);
            return Unauthorized();
        }
    }
}
