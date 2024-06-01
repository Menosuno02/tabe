using ApiTabeAWS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TabeNuget;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoCategoriasController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public ProductoCategoriasController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        // GET: api/ProductoCategorias/{id}
        /// <summary>
        /// Devuelve las categorías de un producto
        /// </summary>
        /// <remarks>
        /// Permite obtener todos las categorías de cierto producto al recibir su ID
        /// </remarks>
        /// <param name="id">ID del producto</param>
        /// <response code="200">Devuelve el conjunto de pedidos del usuario</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<string>>> GetCategoriasFromProducto(int id)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
                return await this.repo.GetCategoriasFromProductoAsync(id);
            return Unauthorized();
        }
    }
}