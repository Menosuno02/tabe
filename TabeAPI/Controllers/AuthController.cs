using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProyectoASPNET;
using ProyectoASPNET.Models;
using System.IdentityModel.Tokens.Jwt;
using TabeAPI.Helpers;
using TabeAPI.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        private HelperActionServicesOAuth helper;

        public AuthController
            (RepositoryRestaurantes repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Usuario usuario = await this.repo.LoginUsuarioAsync(model.Email, model.Password);
            if (usuario == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials =
                    new SigningCredentials(helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);
                JwtSecurityToken token =
                    new JwtSecurityToken(
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token),
                    user = usuario
                });
            }
        }

    }
}
