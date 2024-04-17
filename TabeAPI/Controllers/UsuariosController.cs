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

        // GET: api/Usuarios
        /// <summary>
        /// Devuelve todos los usuarios
        /// </summary>
        /// <remarks>
        /// Permite obtener todos los usuarios de la BBDD
        /// </remarks>
        /// <response code="200">Devuelve el conjunto de usuarios</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Admin</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Usuario>>> GetUsuarios()
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario == 2)
                return await this.repo.GetUsuariosAsync();
            return Unauthorized();
        }

        // GET: api/Usuarios/{id}
        /// <summary>
        /// Busca un usuario
        /// </summary>
        /// <remarks>
        /// Permite obtener un usuario según su ID
        /// </remarks>
        /// <param name="id">ID del usuario</param>
        /// <response code="200">Devuelve el usuario</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Admin o la ID que busca no coincide con su ID logeada</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Usuario>> FindUsuario(int id)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario == 2 || usuario.IdUsuario == id)
                return await this.repo.FindUsuarioAsync(id);
            return Unauthorized();

        }

        // GET: api/GetLoggedUsuario
        /// <summary>
        /// Devuelve el usuario logeado
        /// </summary>
        /// <response code="200">Devuelve el usuario</response>
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

        // GET: api/Usuarios/GetDireccion/{id}
        /// <summary>
        /// Devuelve la dirección del usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <response code="200">Devuelve el usuario</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Admin o Restaurante o la ID que busca no coincide con su ID logeada</response>
        [HttpGet("[action]/{id}")]
        [Authorize]
        public async Task<ActionResult<string>> GetDireccion(int id)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1 || usuario.IdUsuario == id)
                return await this.repo.GetDireccionUsuario(id);
            return Unauthorized();

        }

        // POST: api/Usuarios/RegisterUsuario
        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <remarks>
        /// Permite crear un nuevo usuario
        /// </remarks>
        /// <param name="model">Datos del nuevo usuario a registrar</param>
        /// <response code="200">Devuelve el nuevo usuario</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Usuario>> RegisterUsuario(RegisterUserAPIModel model)
        {
            return await this.repo.RegisterUsuarioAsync(model.Usuario, model.RawPassword);
        }

        // PUT: api/Usuarios
        /// <summary>
        /// Modifica un usuario
        /// </summary>
        /// <remarks>
        /// Permite editar un nuevo usuario
        /// </remarks>
        /// <param name="user">Datos del del nuevo usuario a modificar</param>
        /// <response code="200">Usuario modificado con éxito</response>
        /// <response code="401">No autorizado. El usuario no es Admin y está intentando modificar datos de otro usuario</response>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> EditUsuario(Usuario user)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 2 && usuario.IdUsuario != user.IdUsuario)
                return Unauthorized();
            await this.repo.EditUsuarioAsync(user);
            return Ok();
        }

        // PUT: api/Usuarios/ModificarContrasenya
        /// <summary>
        /// Modifica la contraseña del usuario logeado
        /// </summary>
        /// <param name="model">Nueva y antigua contraseña</param>
        /// <response code="200">Contraseña modificada con éxito</response>
        [HttpPut]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<bool>> ModificarContrasenya(ModifyPasswordAPIModel model)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            return await this.repo.ModificarContrasenyaAsync(usuario, model.OldPassword, model.NewPassword);
        }
    }
}
