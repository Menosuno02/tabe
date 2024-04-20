using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TabeNuget
{
    [Table("CATEGORIAS_RESTAURANTES")]
    public class CategoriaRestaurante
    {
        [Key]
        [Column("IDCATEGORIA")]
        public int IdCategoriaRestaurante { get; set; }
        [Column("NOMBRECATEGORIA")]
        public string Nombre { get; set; }
        [Column("ICONO")]
        public string IconoCategoria { get; set; }
    }
}
