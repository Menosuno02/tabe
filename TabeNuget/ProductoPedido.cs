using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TabeNuget
{
    [Table("PRODUCTOS_PEDIDO")]
    public class ProductoPedido

    {
        [Column("IDPEDIDO")]
        public int IdPedido { get; set; }
        [Column("IDPRODUCTO")]
        public int IdProducto { get; set; }
        [Column("CANTIDAD")]
        public int Cantidad { get; set; }
    }
}
