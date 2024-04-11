namespace ProyectoASPNET.Models
{
    public class ModifyPasswordAPIModel
    {
        public int IdUsuario { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
