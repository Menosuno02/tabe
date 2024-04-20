using Microsoft.EntityFrameworkCore;

namespace TabeNuget
{
    [Keyless]
    public class DistanceMatrixInfo
    {
        public string Distancia { get; set; }
        public int TiempoEstimado { get; set; }
    }
}
