using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoASPNET;
using ProyectoASPNET.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValoracionesRestauranteController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public ValoracionesRestauranteController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpGet("[action]/{idrestaurante}")]
        [Authorize]
        public async Task<ActionResult<ValoracionRestaurante>> ValRestauranteUsuario(int idrestaurante)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            return await this.repo.GetValoracionRestauranteAsync(idrestaurante, usuario.IdUsuario);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult>
            UpdateValoracionRestaurante(ValoracionRestaurante valoracion)
        {
            await this.repo.UpdateValoracionRestauranteAsync(valoracion);
            return Ok();
        }
    }
}
