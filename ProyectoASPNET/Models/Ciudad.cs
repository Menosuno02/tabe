using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET;

[Table("CIUDADES")]
public class Ciudad
{
    [Key]
    [Column("IDCIUDAD")]
    public int IdCiudad { get; set; }
    [Column("NOMBRE")]
    public string Nombre { get; set; }
}
