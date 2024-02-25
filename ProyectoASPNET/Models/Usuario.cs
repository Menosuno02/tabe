using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoASPNET.Models
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("APELLIDOS")]
        public string Apellidos { get; set; }
        [Column("CORREO")]
        public string Correo { get; set; }
        [Column("CONTRASENYA")]
        public string Contrasenya { get; set; }
        [Column("TELEFONO")]
        public string Telefono { get; set; }
        [Column("DIRECCION")]
        public string Direccion { get; set; }
        [Column("TIPOUSUARIO")]
        public string TipoUsuario { get; set; }
        [Column("CIUDAD")]
        public int Ciudad { get; set; }
    }
}
