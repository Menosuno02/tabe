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
        [Column("CIUDAD")]
        public int Ciudad { get; set; }
        [Column("TELEFONO")]
        public string Telefono { get; set; }
        [Column("HORARIO")]
        public string Horario { get; set; }
        [Column("LOGO")]
        public string Logo { get; set; }
        [Column("IMAGEN")]
        public string Imagen { get; set; }
        [Column("TIEMPOENTREGA")]
        public int TiempoEntega { get; set; }
        [Column("IDCATEGORIA")]
        public int IdCategoriaRestaurante { get; set; }
    }
}
