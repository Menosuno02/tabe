using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("V_RESTAURANTES")]
    public class RestauranteView
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
        [Column("NOMBRECATEGORIA")]
        public string CategoriaRestaurante { get; set; }
        [Column("VALORACION")]
        public int Valoracion { get; set; }
        [Column("TIEMPOENTREGA")]
        public string TiempoEntrega { get; set; }
    }
}
