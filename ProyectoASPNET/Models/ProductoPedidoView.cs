using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("V_PRODUCTOS_PEDIDO")]
    public class ProductoPedidoView
    {
        [Column("IDPEDIDO")]
        public int IdPedido { get; set; }
        [Column("IDPRODUCTO")]
        public int IdProducto { get; set; }
        [Column("RESTAURANTE")]
        public string Restaurante { get; set; }
        [Column("ESTADO")]
        public string Estado { get; set; }
        [Column("PRODUCTO")]
        public string Producto { get; set; }
        [Column("PRECIO")]
        public decimal Precio { get; set; }
        [Column("CANTIDAD")]
        public int Cantidad { get; set; }
    }
}
