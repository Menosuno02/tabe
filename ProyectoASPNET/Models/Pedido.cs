using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("PEDIDOS")]
    public class Pedido
    {
        [Key]
        [Column("IDPEDIDO")]
        public int IdPedido { get; set; }
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("IDRESTAURANTE")]
        public int IdRestaurante { get; set; }
        [Column("ESTADO")]
        public int Estado { get; set; }
        [Column("FECHAPEDIDO")]
        public DateTime FechaPedido { get; set; }
        [Column("FECHAENTREGA")]
        public DateTime? FechaEntrega { get; set; }
    }
}
