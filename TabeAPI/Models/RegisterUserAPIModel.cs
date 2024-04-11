using ProyectoASPNET.Models;

namespace TabeAPI.Models
{
    public class RegisterUserAPIModel
    {
        public Usuario Usuario { get; set; }
        public string RawPassword { get; set; }
    }
}
