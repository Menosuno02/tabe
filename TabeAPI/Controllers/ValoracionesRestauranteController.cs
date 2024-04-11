using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{idrestaurante}/{idusuario}")]
        public async Task<ActionResult<ValoracionRestaurante>>
            GetValoracionRestaurante(int idrestaurante, int idusuario)
        {
            return await this.repo.GetValoracionRestauranteAsync
                (idrestaurante, idusuario);
        }

        [HttpPut]
        public async Task<ActionResult>
            UpdateValoracionRestaurante(ValoracionRestaurante valoracion)
        {
            await this.repo.UpdateValoracionRestauranteAsync(valoracion);
            return Ok();
        }
    }
}
