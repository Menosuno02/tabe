using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("CATEGORIAS_RESTAURANTES")]
    public class CategoriaRestaurante
    {
        [Key]
        [Column("IDCATEGORIA")]
        public int IdCategoriaRestaurante { get; set; }
        [Column("NOMBRECATEGORIA")]
        public string Nombre { get; set; }
    }
}
