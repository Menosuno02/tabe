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
    public class CategoriasProductosController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public CategoriasProductosController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpGet("{idrestaurante}")]
        [Authorize]
        public async Task<ActionResult<List<CategoriaProducto>>>
            GetCategoriasProductos(int idrestaurante)
        {
            return await this.repo.GetCategoriasProductosAsync(idrestaurante);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoriaProducto>>
            CreateCategoriaProducto(CategoriaProductoAPIModel model)
        {
            return await this.repo.CreateCategoriaProductoAsync(model.IdRestaurante, model.Categoria);
        }

        [HttpDelete("{idcategoria}")]
        [Authorize]
        public async Task<ActionResult> DeleteCategoriaProducto
            (int idcategoria)
        {
            if (await this.repo.FindProductoAsync(idcategoria) == null) return NotFound();
            else
            {
                await this.repo.DeleteCategoriaProductoAsync(idcategoria);
                return Ok();
            }
        }
    }
}
