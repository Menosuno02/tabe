namespace ProyectoASPNET.Models
{
    public class ProductosActionModel
    {
        public List<Producto> Productos { get; set; }
        public RestauranteView Restaurante { get; set; }
        public List<CategoriaProducto> CategoriasProductos { get; set; }
        public int SelectedCategoria { get; set; }
    }
}
