namespace ProyectoASPNET.Models
{
    public class PaginationRestaurantesView
    {
        public int NumRegistros { get; set; }
        public List<RestauranteView> Restaurantes { get; set; }
    }
}
