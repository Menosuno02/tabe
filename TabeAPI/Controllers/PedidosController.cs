using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoASPNET;
using TabeNuget;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private RepositoryRestaurantes repo;
        private TelemetryClient telemetryClient;

        public PedidosController(RepositoryRestaurantes repo, TelemetryClient telemetryClient)
        {
            this.repo = repo;
            this.telemetryClient = telemetryClient;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Pedido>> CreatePedido(int idrestaurante, List<ProductoCesta> cesta)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 3)
            {
                Pedido pedido = await this.repo.CreatePedidoAsync(usuario.IdUsuario, idrestaurante, cesta);
                List<ProductoPedidoView> productos = await this.repo.GetProductosPedidoViewAsync(new List<int> { pedido.IdPedido });
                this.telemetryClient.TrackEvent("NuevoPedido");
                MetricTelemetry metric = new MetricTelemetry();
                metric.Name = "Nuevo pedido";
                metric.Sum = (double)productos.Sum(p => p.Cantidad * p.Precio);
                this.telemetryClient.TrackMetric(metric);
                string mensaje = usuario.Correo + ": " + metric.Sum + "€";
                TraceTelemetry traza = new TraceTelemetry(mensaje, SeverityLevel.Information);
                this.telemetryClient.TrackTrace(traza);
                return pedido;
            }
            return Unauthorized();
        }
    }
}
