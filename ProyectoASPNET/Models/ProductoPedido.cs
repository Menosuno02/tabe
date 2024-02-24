using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("PRODUCTOS_PEDIDOS")]
    public class ProductoPedido

    {
        [Key]
        [Column("IDPEDIDO")]
        public int IdPedido { get; set; }
        [Key]
        [Column("IDPRODUCTO")]
        public int IdProducto { get; set; }
        [Column("CANTIDAD")]
        public int Cantidad { get; set; }
    }
}
