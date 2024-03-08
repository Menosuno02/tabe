namespace ProyectoASPNET.Models
{
    public class ProductosActionModel
    {
        public RestauranteView Restaurante { get; set; }
        public List<CategoriaProducto> CategoriasProductos { get; set; }
        public int SelectedCategoria { get; set; }
    }
}
