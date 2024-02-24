using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("PRODUCTO_CATEGORIAS")]
    public class ProductoCategorias
    {
        [Key]
        [Column("IDPRODUCTO")]
        public int IdProducto { get; set; }
        [Key]
        [Column("IDCATEGORIA")]
        public int IdCategoria { get; set; }
    }
}
