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
    public class ProductosController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public ProductosController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Producto>>> GetProductos()
        {
            return await this.repo.GetProductosAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Producto>> FindProducto(int id)
        {
            return await this.repo.FindProductoAsync(id);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<ActionResult<List<Producto>>> ProductosRestaurante(int id)
        {
            return await this.repo.GetProductosRestauranteAsync(id);
        }

        [HttpGet]
        [Route("[action]/{restaurante}/{categoria}")]
        [Authorize]
        public async Task<ActionResult<List<Producto>>> ProductosByCategoria
            (int restaurante, int categoria)
        {
            return await this.repo.GetProductosByCategoriaAsync(restaurante, categoria);
        }
        
        [HttpGet("[action]")]
        [Authorize]
        public async Task<ActionResult<List<Producto>>> ListProductos
            ([FromQuery] List<int> idprod)
        {
            return await this.repo.FindListProductosAsync(idprod);
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<int>> GetMaxIdProducto()
        {
            return await this.repo.GetMaxIdProductoAsync();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Producto>> CreateProducto
            (ProductoAPIModel model)
        {
            return await this.repo.CreateProductoAsync(model.Producto, model.CategProducto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> EditProducto
            (ProductoAPIModel model)
        {
            await this.repo.EditProductoAsync(model.Producto, model.CategProducto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            if (await this.repo.FindProductoAsync(id) == null) return NotFound();
            else
            {
                await this.repo.DeleteProductoAsync(id);
                return Ok();
            }
        }
    }
}
