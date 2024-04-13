using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET;
using ProyectoASPNET.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewRestaurantesController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public ViewRestaurantesController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<RestauranteView>>>
            GetRestaurantesView(string? searchquery = "")
        {
            return await this.repo.GetRestaurantesViewAsync(searchquery);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<ActionResult<RestauranteView>>
            FindRestauranteView(int id)
        {
            return await this.repo.FindRestauranteViewAsync(id);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<RestauranteView>>> GetPaginationRestaurantesView
            ([FromQuery] string? searchquery = "")
        {
            return await this.repo.GetPaginationRestaurantesViewAsync(searchquery);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<RestauranteView>>> FilterPaginationRestaurantesView
            ([FromQuery] string categoria, [FromQuery] string? searchquery = "")
        {
            return await this.repo.FilterPaginationRestaurantesViewAsync(categoria, searchquery);
        }
    }
}
