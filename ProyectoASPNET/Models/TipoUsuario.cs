using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("TIPOS_USUARIOS")]
    public class TipoUsuario
    {
        [Key]
        [Column("IDTIPO")]
        public int IdTipoUsuario { get; set; }
        [Column("NOMBRETIPO")]
        public string Nombre { get; set; }
    }
}
