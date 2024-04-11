using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProyectoASPNET.Models
{
    [Table("PRODUCTO_CATEGORIAS")]
    public class ProductoCategorias
    {
        [Column("IDPRODUCTO")]
        public int IdProducto { get; set; }
        [Column("IDCATEGORIA")]
        public int IdCategoria { get; set; }
    }


}
