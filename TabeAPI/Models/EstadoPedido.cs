﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("ESTADOS_PEDIDOS")]
    public class EstadoPedido
    {
        [Key]
        [Column("IDESTADO")]
        public int IdEstado { get; set; }
        [Column("NOMBRE")]
        public string NombreEstado { get; set; }
    }
}
