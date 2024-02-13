namespace ProyectoASPNET.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Contrasenya { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string TipoUsuario { get; set; }
    }
}
