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
    public class ProductosController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public ProductosController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        // GET: api/Productos
        /// <summary>
        /// Devuelve todos los productos
        /// </summary>
        /// <remarks>
        /// Permite obtener todos los productos de la BBDD
        /// </remarks>
        /// <response code="200">Devuelve el conjunto de productos</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Producto>>> GetProductos()
        {
            return await this.repo.GetProductosAsync();
        }

        // GET: api/Productos/{id}
        /// <summary>
        /// Busca un producto
        /// </summary>
        /// <remarks>
        /// Permite obtener un producto según su ID
        /// </remarks>
        /// <param name="id">ID del producto</param>
        /// <response code="200">Devuelve el producto</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Producto>> FindProducto(int id)
        {
            return await this.repo.FindProductoAsync(id);
        }

        // GET: api/Productos/ProductosRestaurante/{id}
        /// <summary>
        /// Busca los productos de un restaurante
        /// </summary>
        /// <remarks>
        /// Permite obtener los productos de un restaurante recibiendo el ID de este
        /// </remarks>
        /// <param name="id">ID del restaurante</param>
        /// <response code="200">Devuelve el conjunto de productos</response>
        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<ActionResult<List<Producto>>> ProductosRestaurante(int id)
        {
            return await this.repo.GetProductosRestauranteAsync(id);
        }

        // GET: api/Productos/ProductosRestaurante/{id}
        /// <summary>
        /// Busca los productos de cierto restaurante y categoría
        /// </summary>
        /// <remarks>
        /// Permite obtener los productos de un restaurante y una categoría de este con el ID de ambos
        /// </remarks>
        /// <param name="restaurante">ID del restaurante</param>
        /// <param name="categoria">ID de la categoría</param>
        /// <response code="200">Devuelve el conjunto de productos</response>
        [HttpGet]
        [Route("[action]/{restaurante}/{categoria}")]
        [Authorize]
        public async Task<ActionResult<List<Producto>>> ProductosByCategoria(int restaurante, int categoria)
        {
            return await this.repo.GetProductosByCategoriaAsync(restaurante, categoria);
        }

        // GET: api/Productos/ListProductos/{id}
        /// <summary>
        /// Devuelve los productos indicados
        /// </summary>
        /// <remarks>
        /// Permite obtener los productos que se encuentren en la lista de IDs recibida
        /// </remarks>
        /// <param name="idprod">IDs de los productos</param>
        /// <response code="200">Devuelve el conjunto de productos</response>
        [HttpGet("[action]")]
        [Authorize]
        public async Task<ActionResult<List<Producto>>> ListProductos([FromQuery] List<int> idprod)
        {
            return await this.repo.FindListProductosAsync(idprod);
        }

        // POST: api/Productos
        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <param name="model">Datos del nuevo producto + sus categorías</param>
        /// <response code="200">Devuelve el nuevo producto</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin, o es un Restaurante intentando crear un producto para otro restaurante</response>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Producto>> CreateProducto(ProductoAPIModel model)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
            {
                if (usuario.TipoUsuario == 3)
                {
                    Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(usuario.IdUsuario);
                    if (rest.IdRestaurante != model.Producto.IdRestaurante)
                        return Unauthorized();
                }
                model.Producto.Imagen = "img" + model.Producto.IdProducto + ".jpeg";
                return await this.repo.CreateProductoAsync(model.Producto, model.CategProducto);
            }
            return Unauthorized();
        }

        // PUT: api/Productos
        /// <summary>
        /// Modifica un producto
        /// </summary>
        /// <param name="model">Datos nuevos del producto + nuevas categorías</param>
        /// <response code="200">Producto modificado con éxito</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin, o es un Restaurante intentando modificar un producto de otro restaurante</response>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> EditProducto(ProductoAPIModel model)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
            {
                if (usuario.TipoUsuario == 3)
                {
                    Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(usuario.IdUsuario);
                    if (rest.IdRestaurante != model.Producto.IdRestaurante)
                        return Unauthorized();
                }
                model.Producto.Imagen = "img" + model.Producto.IdProducto + ".jpeg";
                await this.repo.EditProductoAsync(model.Producto, model.CategProducto);
                return Ok();
            }
            return Unauthorized();
        }

        // DELETE: api/Productos/{id}
        /// <summary>
        /// Elimina un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <response code="200">Producto eliminado con éxito</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante o Admin</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1)
            {
                Producto producto = await this.repo.FindProductoAsync(id);
                if (producto == null)
                    return NotFound();
                if (usuario.TipoUsuario == 3)
                {
                    Restaurante rest = await this.repo.GetRestauranteFromLoggedUserAsync(usuario.IdUsuario);
                    if (rest.IdRestaurante != producto.IdRestaurante)
                        return Unauthorized();
                }
                await this.repo.DeleteProductoAsync(id);
                return Ok();
            }
            return Unauthorized();
        }
    }
}
