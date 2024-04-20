using TabeNuget;

namespace ProyectoASPNET.Services
{
    public interface IServiceRestaurantes
    {
        #region RESTAURANTES
        public Task<List<Restaurante>> GetRestaurantesAsync();

        public Task<Restaurante> FindRestauranteAsync(int id);

        public Task<Restaurante> CreateRestauranteAsync(Restaurante restaurante, string password, IFormFile imagen);

        public Task EditRestauranteAsync(Restaurante restaurante, IFormFile imagen);

        public Task DeleteRestauranteAsync(int id);

        public Task<Restaurante> GetRestauranteFromLoggedUserAsync();
        #endregion

        #region V_RESTAURANTES
        public Task<List<RestauranteView>> GetRestaurantesViewAsync(string searchquery);

        public Task<RestauranteView> FindRestauranteViewAsync(int id);

        public Task<List<RestauranteView>> FilterRestaurantesViewAsync(string categoria, string searchquery);
        #endregion

        #region CATEGORIAS_RESTAURANTES
        public Task<List<CategoriaRestaurante>> GetCategoriasRestaurantesAsync();
        #endregion

        #region CATEGORIAS_PRODUCTOS
        public Task<List<CategoriaProducto>> GetCategoriasProductosAsync(int idrestaurante);

        public Task<CategoriaProducto> CreateCategoriaProductoAsync(int idrestaurante, string categoria);

        public Task DeleteCategoriaProductoAsync(int idcategoria);
        #endregion

        #region PRODUCTO_CATEGORIAS
        public Task<List<string>> GetCategoriasFromProductoAsync(int idprod);
        #endregion

        #region PRODUCTOS
        public Task<List<Producto>> GetProductosAsync();

        public Task<List<Producto>> GetProductosRestauranteAsync(int id);

        public Task<List<Producto>> GetProductosByCategoriaAsync(int restaurante, int categoria);

        public Task<Producto> FindProductoAsync(int id);

        public Task<List<Producto>> FindListProductosAsync(IEnumerable<int> ids);

        public Task<Producto> CreateProductoAsync(Producto producto, int[] categproducto, IFormFile imagen);

        public Task EditProductoAsync(Producto producto, int[] categproducto, IFormFile imagen);

        public Task DeleteProductoAsync(int id);
        #endregion

        #region USUARIOS
        public Task<Usuario> LoginUsuarioAsync(string email, string password);

        public Task<Usuario> RegisterUsuarioAsync(Usuario user, string password);

        public Task<List<Usuario>> GetUsuariosAsync();

        public Task<Usuario> FindUsuarioAsync(int id);

        public Task EditUsuarioAsync(Usuario user);

        public Task<bool> ModificarContrasenyaAsync(string actual, string nueva);

        public Task<string> GetDireccionUsuario(int idusuario);
        #endregion

        #region PEDIDOS
        public Task<Pedido> CreatePedidoAsync(int idrestaurante, List<ProductoCesta> cesta);

        public Task<List<Pedido>> GetPedidosUsuarioAsync();

        public Task<List<Pedido>> GetPedidosRestauranteAsync();
        #endregion

        #region ESTADOS_PEDIDO
        public Task<List<EstadoPedido>> GetEstadoPedidosAsync();

        public Task UpdateEstadoPedidoAsync(int idpedido, int estado);
        #endregion

        #region V_PRODUCTOS_PEDIDO
        public Task<List<ProductoPedidoView>> GetProductosPedidoViewAsync(List<int> idpedidos);
        #endregion

        #region VALORACIONES_RESTAURANTE
        public Task<ValoracionRestaurante> GetValoracionRestauranteAsync(int idrestaurante);

        public Task UpdateValoracionRestauranteAsync(ValoracionRestaurante val);
        #endregion
    }
}
