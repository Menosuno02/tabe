using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using TabeAPI.Models;
using ProyectoASPNET.Models;
using ProyectoASPNET.Helpers;

namespace ProyectoASPNET.Services
{
    public class ServiceApiRestaurantes : IServiceRestaurantes
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue Header;
        private string EncryptKey;
        private HelperUploadFiles helperUploadFiles;
        private HelperCryptography helperCryptography;
        private IHttpContextAccessor httpContextAccessor;

        public ServiceApiRestaurantes
            (IConfiguration configuration,
            HelperUploadFiles helperUploadFiles,
            HelperCryptography helperCryptography,
            IHttpContextAccessor httpContextAccessor)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:TabeApi");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.EncryptKey = configuration.GetValue<string>("EncryptKey");
            this.helperUploadFiles = helperUploadFiles;
            this.helperCryptography = helperCryptography;
            this.httpContextAccessor = httpContextAccessor;
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else return default(T);
            }
        }

        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else return default(T);
            }
        }

        #region RESTAURANTES
        public async Task<List<Restaurante>> GetRestaurantesAsync()
        {
            string request = "api/Restaurantes";
            return await this.CallApiAsync<List<Restaurante>>(request);
        }

        public async Task<Restaurante> FindRestauranteAsync(int id)
        {
            string request = "api/Restaurantes/FindRestaurante/" + id;
            return await this.CallApiAsync<Restaurante>(request);
        }

        public async Task<Restaurante> CreateRestauranteAsync(Restaurante restaurante, string password, IFormFile imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Restaurantes";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                restaurante.IdRestaurante = await this.CallApiAsync<int>("api/Restaurantes/GetMaxIdRestaurante");
                restaurante.Imagen = await helperUploadFiles.UploadFileAsync(imagen, Folders.ImagRestaurantes, restaurante.IdRestaurante);
                RestauranteAPIModel model = new RestauranteAPIModel
                {
                    Restaurante = restaurante,
                    Password = password,
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                return await response.Content.ReadAsAsync<Restaurante>();
            }
        }

        public async Task EditRestauranteAsync(Restaurante restaurante, IFormFile imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Restaurantes";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                if (imagen != null)
                {
                    restaurante.Imagen =
                        await helperUploadFiles.UploadFileAsync(imagen, Folders.ImagRestaurantes, restaurante.IdRestaurante);
                }
                string json = JsonConvert.SerializeObject(restaurante);
                StringContent context = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, context);
            }
        }

        public async Task DeleteRestauranteAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Restaurantes/" + id;
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task<Restaurante> GetRestauranteFromLoggedUserAsync()
        {
            string request = "api/Restaurantes/GetRestauranteFromLoggedUser";
            return await this.CallApiAsync<Restaurante>(request);
        }

        public async Task<Usuario> GetUsuarioFromRestauranteAsync(string restCorreo)
        {
            string request = "api/Restaurantes/GetUsuarioFromRestaurante/" + restCorreo;
            return await this.CallApiAsync<Usuario>(request);
        }
        #endregion

        #region V_RESTAURANTES
        public async Task<List<RestauranteView>> GetRestaurantesViewAsync(string searchquery)
        {
            string request = "api/ViewRestaurantes";
            if (searchquery != "") request += "?searchquery=" + searchquery;
            return await this.CallApiAsync<List<RestauranteView>>(request);
        }

        public async Task<RestauranteView> FindRestauranteViewAsync(int id)
        {
            string request = "api/ViewRestaurantes/FindRestauranteView/" + id;
            return await this.CallApiAsync<RestauranteView>(request);
        }

        public async Task<List<RestauranteView>> GetPaginationRestaurantesViewAsync(string searchquery)
        {
            string request = "api/ViewRestaurantes/GetPaginationRestaurantesView?";
            if (searchquery != "") request += "searchquery=" + searchquery;
            return await this.CallApiAsync<List<RestauranteView>>(request);
        }

        public async Task<List<RestauranteView>> FilterPaginationRestaurantesViewAsync(string categoria, string searchquery)
        {
            string request = "api/ViewRestaurantes/FilterPaginationRestaurantesView?categoria=" + categoria + "&";
            if (searchquery != "") request += "searchquery=" + searchquery;
            return await this.CallApiAsync<List<RestauranteView>>(request);
        }
        #endregion

        #region CATEGORIAS_RESTAURANTES
        public async Task<List<CategoriaRestaurante>> GetCategoriasRestaurantesAsync()
        {
            string request = "api/CategoriasRestaurantes";
            return await this.CallApiAsync<List<CategoriaRestaurante>>(request);
        }
        #endregion

        #region CATEGORIAS_PRODUCTOS
        public async Task<List<CategoriaProducto>> GetCategoriasProductosAsync(int idrestaurante)
        {
            string request = "api/CategoriasProductos/" + idrestaurante;
            return await this.CallApiAsync<List<CategoriaProducto>>(request);
        }

        public async Task<CategoriaProducto> CreateCategoriaProductoAsync(int idrestaurante, string categoria)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/CategoriasProductos";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                CategoriaProductoAPIModel model = new CategoriaProductoAPIModel
                {
                    IdRestaurante = idrestaurante,
                    Categoria = categoria
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                return await response.Content.ReadAsAsync<CategoriaProducto>();
            }
        }

        public async Task DeleteCategoriaProductoAsync(int idcategoria)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/CategoriasProductos/" + idcategoria;
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.DeleteAsync(request);
            }
        }
        #endregion

        #region PRODUCTO_CATEGORIAS
        public async Task<List<string>> GetCategoriasFromProductoAsync(int idprod)
        {
            string request = "api/ProductoCategorias/" + idprod;
            return await this.CallApiAsync<List<string>>(request);
        }
        #endregion

        #region PRODUCTOS
        public async Task<List<Producto>> GetProductosAsync()
        {
            string request = "api/Productos";
            return await this.CallApiAsync<List<Producto>>(request);
        }

        public async Task<List<Producto>> GetProductosRestauranteAsync(int id)
        {
            string request = "api/Productos/ProductosRestaurante/" + id;
            return await this.CallApiAsync<List<Producto>>(request);
        }

        public async Task<List<Producto>> GetProductosByCategoriaAsync(int restaurante, int categoria)
        {
            string request = "api/Productos/ProductosByCategoria/" + restaurante + "/" + categoria;
            return await this.CallApiAsync<List<Producto>>(request);
        }

        public async Task<Producto> FindProductoAsync(int id)
        {
            string request = "api/Productos/" + id;
            return await this.CallApiAsync<Producto>(request);
        }

        public async Task<List<Producto>> FindListProductosAsync(IEnumerable<int> ids)
        {
            string request = "api/Productos/ListProductos?";
            foreach (int id in ids)
                request += "idprod=" + id + "&";
            request = request.TrimEnd('&');
            if (ids.Count() == 0) request = request.TrimEnd('?');
            return await this.CallApiAsync<List<Producto>>(request);
        }

        public async Task<Producto> CreateProductoAsync(Producto producto, int[] categproducto, IFormFile imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Productos";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                producto.IdProducto = await this.CallApiAsync<int>("api/Productos/GetMaxIdProducto");
                producto.Imagen = await this.helperUploadFiles.UploadFileAsync(imagen, Folders.ImagProductos, producto.IdProducto);
                ProductoAPIModel model = new ProductoAPIModel
                {
                    Producto = producto,
                    CategProducto = categproducto,
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                return await response.Content.ReadAsAsync<Producto>();
            }
        }

        public async Task EditProductoAsync(Producto producto, int[] categproducto, IFormFile imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Productos";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                if (imagen != null)
                {
                    producto.Imagen =
                        await helperUploadFiles.UploadFileAsync(imagen, Folders.ImagProductos, producto.IdProducto);
                }
                ProductoAPIModel model = new ProductoAPIModel
                {
                    Producto = producto,
                    CategProducto = categproducto,
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, content);
            }
        }

        public async Task DeleteProductoAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Productos/" + id;
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.DeleteAsync(request);
            }
        }
        #endregion

        #region USUARIOS
        public async Task<Usuario> LoginUsuarioAsync(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Auth/Login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    Email = email,
                    Password = password
                };
                string jsonData = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent
                    (jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    HttpContext httpContext = this.httpContextAccessor.HttpContext;
                    httpContext.Session.SetString("TOKEN", helperCryptography.EncryptString(this.EncryptKey, token));
                    return await this.CallApiAsync<Usuario>("api/Usuarios/GetLoggedUsuario", token);
                }
                else return null;
            }
        }

        public async Task<Usuario> RegisterUsuarioAsync(Usuario user, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Usuarios/RegisterUsuario";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                user.Salt = "";
                user.Contrasenya = new byte[] { };
                RegisterUserAPIModel model = new RegisterUserAPIModel
                {
                    Usuario = user,
                    RawPassword = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent context = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, context);
                string jsonUsuario = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            }
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            string request = "api/Usuarios";
            return await this.CallApiAsync<List<Usuario>>(request);
        }

        public async Task<Usuario> FindUsuarioAsync(int id)
        {
            string request = "api/Usuarios/" + id;
            return await this.CallApiAsync<Usuario>(request);
        }

        public async Task EditUsuarioAsync(Usuario user)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Usuarios";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                user.Contrasenya = new byte[] { };
                string json = JsonConvert.SerializeObject(user);
                StringContent context = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, context);
            }
        }

        public async Task<bool> ModificarContrasenyaAsync(Usuario usu, string actual, string nueva)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Usuarios/ModificarContrasenya";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                ModifyPasswordAPIModel model = new ModifyPasswordAPIModel
                {
                    IdUsuario = usu.IdUsuario,
                    NewPassword = nueva,
                    OldPassword = actual
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent context = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, context);
                return await response.Content.ReadAsAsync<bool>();
            }
        }
        #endregion

        #region PEDIDOS
        public async Task<Pedido> CreatePedidoAsync
            (int idrestaurante, List<ProductoCesta> cesta)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Pedidos/" + idrestaurante;
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                string json = JsonConvert.SerializeObject(cesta);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                return await response.Content.ReadAsAsync<Pedido>();
            }
        }

        public async Task<List<Pedido>> GetPedidosUsuarioAsync()
        {
            string request = "api/Pedidos/GetPedidosUsuario";
            return await this.CallApiAsync<List<Pedido>>(request);
        }

        public async Task<List<Pedido>> GetPedidosRestauranteAsync()
        {
            string request = "api/Pedidos/GetPedidosRestaurante";
            return await this.CallApiAsync<List<Pedido>>(request);
        }
        #endregion

        #region ESTADOS_PEDIDO
        public async Task<List<EstadoPedido>> GetEstadoPedidosAsync()
        {
            string request = "api/EstadosPedido";
            return await this.CallApiAsync<List<EstadoPedido>>(request);
        }

        public async Task UpdateEstadoPedidoAsync(int idpedido, int estado)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/EstadosPedido/UpdateEstadoPedido";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                EstadoPedidoAPIModel model = new EstadoPedidoAPIModel
                {
                    IdPedido = idpedido,
                    Estado = estado
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent context = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, context);
            }
        }
        #endregion

        #region V_PRODUCTOS_PEDIDO
        public async Task<List<ProductoPedidoView>> GetProductosPedidoViewAsync(List<int> idpedidos)
        {
            string request = "api/ProductosPedidoView?";
            foreach (int id in idpedidos)
                request += "idpedido=" + id + "&";
            request = request.TrimEnd('&');
            if (idpedidos.Count() == 0) request = request.TrimEnd('?');
            return await this.CallApiAsync<List<ProductoPedidoView>>(request);
        }
        #endregion

        #region VALORACIONES_RESTAURANTE
        public async Task<ValoracionRestaurante> GetValoracionRestauranteAsync(int idrestaurante)
        {
            string request = "api/ValoracionesRestaurante/ValRestauranteUsuario/" + idrestaurante;
            return await this.CallApiAsync<ValoracionRestaurante>(request);
        }

        public async Task UpdateValoracionRestauranteAsync(ValoracionRestaurante val)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/ValoracionesRestaurante";
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string token = httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "TOKEN").Value;
                token = helperCryptography.DecryptString(this.EncryptKey, token);
                if (token != null)
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                string json = JsonConvert.SerializeObject(val);
                StringContent context = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, context);
            }
        }
        #endregion
    }
}
