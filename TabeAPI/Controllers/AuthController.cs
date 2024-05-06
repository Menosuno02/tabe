using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProyectoASPNET;
using TabeNuget;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TabeAPI.Helpers;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryRestaurantes repo;
        private HelperActionServicesOAuth helper;

        public AuthController(RepositoryRestaurantes repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        // POST: api/Auth/Login
        /// <summary>
        /// Devuelve un token para usar la API al iniciar sesión con correo y contraseña
        /// </summary>
        /// <remarks>
        /// Permite iniciar sesión con un correo y una contraseña. Si son válidos, devolverá un token que permitirá usar el resto de la API
        /// </remarks>
        /// <param name="model">LoginModel, contiene el correo y la contraseña introducidos</param>
        /// <response code="200">Devuelve el token</response>
        /// <response code="401">No autorizado, inicio de sesión inválido</response>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Usuario usuario = await this.repo.LoginUsuarioAsync(model.Email, model.Password);
            if (usuario == null)
                return Unauthorized();
            else
            {
                SigningCredentials credentials =
                    new SigningCredentials(helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);
                string json = JsonConvert.SerializeObject(usuario);
                Claim[] info = new Claim[]
                {
                    new Claim("UserData", json)
                };
                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: info,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token),
                });
            }
        }

    }
}
