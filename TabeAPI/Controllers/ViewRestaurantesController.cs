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
        [Authorize]
        public async Task<ActionResult<PaginationRestaurantesView>>
            GetPaginationRestaurantesView(string? searchquery = "", int posicion = 1)
        {
            return await this.repo.GetPaginationRestaurantesViewAsync(searchquery, posicion);
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<PaginationRestaurantesView>>
            FilterPaginationRestaurantesView(string categoria, string? searchquery = "", int posicion = 1)
        {
            return await this.repo.FilterPaginationRestaurantesViewAsync(categoria, searchquery, posicion);
        }
    }
}
