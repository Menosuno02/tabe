using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("PRODUCTOS")]
    public class Producto
    {
        [Key]
        [Column("IDPRODUCTO")]
        public int IdProducto { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }
        [Column("PRECIO")]
        public double Precio { get; set; }
        [Column("IMAGEN")]
        public string Imagen { get; set; }
        [Column("IDRESTAURANTE")]
        public int IdRestaurante { get; set; }
    }
}
