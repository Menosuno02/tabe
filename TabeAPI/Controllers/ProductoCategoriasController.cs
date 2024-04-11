using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<List<string>>>
            GetCategoriasFromProducto(int id)
        {
            return await this.repo.GetCategoriasFromProductoAsync(id);
        }
    }
}
