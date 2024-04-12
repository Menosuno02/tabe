using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET;
using ProyectoASPNET.Models;
using TabeAPI.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantesController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public RestaurantesController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Restaurante>>> GetRestaurantes()
        {
            return await this.repo.GetRestaurantesAsync();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult<Restaurante>> FindRestaurante(int id)
        {
            return await this.repo.FindRestauranteAsync(id);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult<Restaurante>>
            GetRestauranteFromLoggedUser(int id)
        {
            return await this.repo.GetRestauranteFromLoggedUserAsync(id);
        }

        [HttpGet]
        [Route("[action]/{restaurantecorreo}")]
        public async Task<ActionResult<Usuario>>
            GetUsuarioFromRestaurante(string restaurantecorreo)
        {
            return await this.repo.GetUsuarioFromRestauranteAsync(restaurantecorreo);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<int>> GetMaxIdRestaurante()
        {
            return await this.repo.GetMaxIdRestauranteAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Restaurante>> CreateRestaurante
            (RestauranteAPIModel model)
        {
            return await this.repo.CreateRestauranteAsync(model.Restaurante, model.Password);
        }

        [HttpPut]
        public async Task<ActionResult<Restaurante>> EditRestaurante(Restaurante restaurante)
        {
            return await this.repo.EditRestauranteAsync(restaurante);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRestaurante(int id)
        {
            if (await this.repo.FindRestauranteAsync(id) == null) return NotFound();
            else
            {
                await this.repo.DeleteRestauranteAsync(id);
                return Ok();
            }
        }
    }
}
