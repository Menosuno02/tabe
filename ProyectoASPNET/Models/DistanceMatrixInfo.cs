using Microsoft.EntityFrameworkCore;

namespace ProyectoASPNET.Models
{
    [Keyless]
    public class DistanceMatrixInfo
    {
        public string Distancia { get; set; }
        public int TiempoEstimado { get; set; }
    }
}
