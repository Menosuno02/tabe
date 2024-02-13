namespace ProyectoASPNET.Models
{
    public class Restaurante
    {
        public int IdRestaurante { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Descripcion { get; set; }
        public string Horario { get; set; }
        public string Logo { get; set; }
        public string Imagen { get; set; }
        public int TiempoEntega { get; set; }
        public int IdCategoriaRestaurante { get; set; }
    }
}
