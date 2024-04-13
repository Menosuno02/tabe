using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoASPNET;
using ProyectoASPNET.Models;
using System.Security.Claims;
using TabeAPI.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public UsuariosController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Usuario>>>
            GetUsuarios()
        {
            return await this.repo.GetUsuariosAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Usuario>> FindUsuario(int id)
        {
            return await this.repo.FindUsuarioAsync(id);
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<Usuario>> GetLoggedUsuario()
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            return usuario;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Usuario>> RegisterUsuario(RegisterUserAPIModel model)
        {
            return await this.repo.RegisterUsuarioAsync(model.Usuario, model.RawPassword);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> EditUsuario(Usuario user)
        {
            await this.repo.EditUsuarioAsync(user);
            return Ok();
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<bool>> ModificarContrasenya(ModifyPasswordAPIModel model)
        {
            Usuario usu = await this.repo.FindUsuarioAsync(model.IdUsuario);
            return await this.repo.ModificarContrasenyaAsync(usu, model.OldPassword, model.NewPassword);
        }
    }
}
