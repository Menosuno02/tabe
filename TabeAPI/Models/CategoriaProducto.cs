using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("CATEGORIAS_PRODUCTOS")]
    public class CategoriaProducto
    {
        [Key]
        [Column("IDCATEGORIA")]
        public int IdCategoriaProducto { get; set; }
        [Column("IDRESTAURANTE")]
        public int IdRestaurante { get; set; }
        [Column("NOMBRECATEGORIA")]
        public string Nombre { get; set; }
    }
}
