using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET;
using TabeNuget;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasRestaurantesController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public CategoriasRestaurantesController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        // GET: api/CategoriasRestaurantes
        /// <summary>
        /// Devuelve todas las categorías para restaurantes
        /// </summary>
        /// <remarks>
        /// Permite obtener las categorías para restaurantes de la BBDD
        /// </remarks>
        /// <response code="200">Devuelve el conjunto de categorías para los restaurantes</response>
        /// <response code="401">No autorizado. No se ha iniciado sesión</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<CategoriaRestaurante>>> GetCategoriasRestaurantes()
        {
            return await this.repo.GetCategoriasRestaurantesAsync();
        }
    }
}
