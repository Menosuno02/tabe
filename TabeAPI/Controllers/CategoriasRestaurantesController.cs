using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET;
using ProyectoASPNET.Models;

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

        [HttpGet]
        public async Task<ActionResult<List<CategoriaRestaurante>>>
            GetCategoriasRestaurantes()
        {
            return await this.repo.GetCategoriasRestaurantesAsync();
        }
    }
}
