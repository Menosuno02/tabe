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

        // GET: api/ValoracionesRestaurante/ValRestauranteUsuario/{idrestaurante}
        /// <summary>
        /// Devuelve la valoración a un restaurante del usuario logeado
        /// </summary>
        /// <remarks>
        /// Permite obtener la valoración a un restaurante del usuario logeado
        /// </remarks>
        /// <param name="idrestaurante">ID del restaurante</param>
        /// <response code="200">Devuelve la valoración</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Usuario o Admin</response>
        [HttpGet("[action]/{idrestaurante}")]
        [Authorize]
        public async Task<ActionResult<ValoracionRestaurante>> ValRestauranteUsuario(int idrestaurante)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 3)
                return await this.repo.GetValoracionRestauranteAsync(idrestaurante, usuario.IdUsuario);
            return Unauthorized();
        }

        // PUT: api/ValoracionesRestaurante/UpdateValoracionRestaurante
        /// <summary>
        /// Modifica la valoración a un restaurante
        /// </summary>
        /// <remarks>
        /// Permite editar la valoración realizada a un restaurante
        /// </remarks>
        /// <param name="valoracion">ID del restaurante, ID del usuario y valoración</param>
        /// <response code="200">Valoración modificada</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Usuario o Admin o es un Usuario intentando modificar la valoración de otro usuario</response>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateValoracionRestaurante(ValoracionRestaurante valoracion)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 3)
            {
                if (usuario.TipoUsuario == 1 && usuario.IdUsuario != valoracion.IdUsuario)
                    return Unauthorized();
                await this.repo.UpdateValoracionRestauranteAsync(valoracion);
                return Ok();
            }
            return Unauthorized();
        }
    }
}
