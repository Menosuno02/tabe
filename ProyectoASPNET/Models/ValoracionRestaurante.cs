using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("VALORACIONES_RESTAURANTE")]
    public class ValoracionRestaurante
    {
        [Key]
        [Column("IDRESTAURANTE")]
        public int IdRestaurante { get; set; }
        [Key]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("VALORACION")]
        public int Valoracion { get; set; }
    }
}
