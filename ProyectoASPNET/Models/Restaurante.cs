using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("RESTAURANTES")]
    public class Restaurante
    {
        [Key]
        [Column("IDRESTAURANTE")]
        public int IdRestaurante { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("DIRECCION")]
        public string Direccion { get; set; }
        [Column("TELEFONO")]
        public string Telefono { get; set; }
        [Column("IMAGEN")]
        public string Imagen { get; set; }
        [Column("IDCATEGORIA")]
        public int CategoriaRestaurante { get; set; }
        [Column("TIEMPOENTREGA")]
        public string TiempoEntrega { get; set; }
    }
}
