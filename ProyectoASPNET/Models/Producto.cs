namespace ProyectoASPNET.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public string Imagen { get; set; }
        public int IdRestaurante { get; set; }
        public int IdCategoriaProducto { get; set; }
    }
}
