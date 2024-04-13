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
    public class RestaurantesController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public RestaurantesController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Restaurante>>> GetRestaurantes()
        {
            return await this.repo.GetRestaurantesAsync();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<ActionResult<Restaurante>> FindRestaurante(int id)
        {
            return await this.repo.FindRestauranteAsync(id);
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<Restaurante>>
            GetRestauranteFromLoggedUser()
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            return await this.repo.GetRestauranteFromLoggedUserAsync(usuario.IdUsuario);
        }

        [HttpGet]
        [Route("[action]/{restaurantecorreo}")]
        [Authorize]
        public async Task<ActionResult<Usuario>>
            GetUsuarioFromRestaurante(string restaurantecorreo)
        {
            return await this.repo.GetUsuarioFromRestauranteAsync(restaurantecorreo);
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<int>> GetMaxIdRestaurante()
        {
            return await this.repo.GetMaxIdRestauranteAsync();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Restaurante>> CreateRestaurante
            (RestauranteAPIModel model)
        {
            return await this.repo.CreateRestauranteAsync(model.Restaurante, model.Password);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Restaurante>> EditRestaurante(Restaurante restaurante)
        {
            return await this.repo.EditRestauranteAsync(restaurante);
        }

        [HttpDelete("{id}")]
        [Authorize]
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
