using Microsoft.EntityFrameworkCore;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Data
{
    public class RestaurantesContext : DbContext
    {
        public RestaurantesContext(DbContextOptions<RestaurantesContext> options) : base(options) { }

        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<CategoriaRestaurante> CategoriaRestaurantes { get; set; }
        public DbSet<EstadoPedido> EstadoPedidos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoCategorias> ProductoCategorias { get; set; }
        public DbSet<ProductoPedido> ProductoPedidos { get; set; }
        public DbSet<RestauranteView> RestaurantesView { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ValoracionRestaurante> ValoracionRestaurantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductoCategorias>()
                .HasKey(pc => new { pc.IdProducto, pc.IdCategoria });
            modelBuilder.Entity<ProductoPedido>()
                .HasKey(pp => new { pp.IdPedido, pp.IdProducto });
            modelBuilder.Entity<ValoracionRestaurante>()
                .HasKey(vr => new { vr.IdRestaurante, vr.IdUsuario });
        }
    }
}
