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
    public class CategoriasProductosController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public CategoriasProductosController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        // GET: api/CategoriasProductos/{idrestaurante}
        /// <summary>
        /// Devuelve todas las categorías para productos de un restaurante
        /// </summary>
        /// <remarks>
        /// Recibe el ID de un restaurante y permite obtener todas sus categorías para sus productos
        /// </remarks>
        /// <param name="idrestaurante">ID del restaurante</param>
        /// <response code="200">Devuelve el conjunto de categorías</response>
        [HttpGet("{idrestaurante}")]
        [Authorize]
        public async Task<ActionResult<List<CategoriaProducto>>> GetCategoriasProductos(int idrestaurante)
        {
            return await this.repo.GetCategoriasProductosAsync(idrestaurante);
        }

        // POST: api/CategoriasProductos
        /// <summary>
        /// Crea una nueva categoría para productos
        /// </summary>
        /// <remarks>
        /// Permite crear una nueva categoría para productos con el ID del restaurante y el nombre de la nueva categoría
        /// </remarks>
        /// <param name="model">ID del restaurante de la categoría y nombre de la categoría</param>
        /// <response code="200">Devuelve el objeto creado</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin, o es un Restaurante intentando crear una categoría para otro restaurante</response>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoriaProducto>> CreateCategoriaProducto(CategoriaProductoAPIModel model)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
            {
                if (usuario.TipoUsuario == 3)
                {
                    Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(usuario.IdUsuario);
                    if (rest.IdRestaurante != model.IdRestaurante) return Unauthorized();
                }
                return await this.repo.CreateCategoriaProductoAsync(model.IdRestaurante, model.Categoria);
            }
            return Unauthorized();
        }

        // DELETE: api/CategoriasProductos/{idcategoria}
        /// <summary>
        /// Elimina una categoría para productos
        /// </summary>
        /// <remarks>
        /// Recibe el ID de una categoría para productos y permite eliminar esta categoría
        /// </remarks>
        /// <param name="idcategoria">ID de la categoría</param>
        /// <response code="200">Categoría borrada con éxito</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin, o es un Restaurante intentando borrar una categoría de otro restaurante</response>
        /// <response code="404">Categoría no encontrada</response>
        [HttpDelete("{idcategoria}")]
        [Authorize]
        public async Task<ActionResult> DeleteCategoriaProducto(int idcategoria)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
            {
                CategoriaProducto categ = await this.repo.FindCategoriaProductoAsync(idcategoria);
                if (categ == null)
                    return NotFound();
                if (usuario.TipoUsuario == 3)
                {
                    Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(usuario.IdUsuario);
                    if (rest.IdRestaurante != categ.IdRestaurante) return Unauthorized();
                }
                await this.repo.DeleteCategoriaProductoAsync(idcategoria);
                return Ok();
            }
            return Unauthorized();
        }
    }
}
