using Microsoft.EntityFrameworkCore;
using TabeNuget;

namespace ProyectoASPNET.Data
{
    public class RestaurantesContext : DbContext
    {
        public RestaurantesContext(DbContextOptions<RestaurantesContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Restaurante> Restaurantes { get; set; }
    }
}
