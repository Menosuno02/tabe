﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using TabeAPI.Models;
using ProyectoASPNET.Models;
using ProyectoASPNET.Helpers;
using System.Numerics;

namespace ProyectoASPNET.Services
{
    public class ServiceApiRestaurantes : IServiceRestaurantes
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue Header;
        private HelperUploadFiles helperUploadFiles;
        private IHttpContextAccessor httpContextAccessor;

        public ServiceApiRestaurantes
            (IConfiguration configuration,
            HelperUploadFiles helperUploadFiles,
            IHttpContextAccessor httpContextAccessor)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:TabeApi");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.helperUploadFiles = helperUploadFiles;
            this.httpContextAccessor = httpContextAccessor;
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
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
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
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
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task<Restaurante> GetRestauranteFromLoggedUserAsync(int id)
        {
            string request = "api/Restaurantes/GetRestauranteFromLoggedUser/" + id;
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
            string request = "api/ViewRestaurantes/" + searchquery;
            return await this.CallApiAsync<List<RestauranteView>>(request);
        }

        public async Task<RestauranteView> FindRestauranteViewAsync(int id)
        {
            string request = "api/ViewRestaurantes/FindRestauranteView/" + id;
            return await this.CallApiAsync<RestauranteView>(request);
        }

        public async Task<PaginationRestaurantesView> GetPaginationRestaurantesViewAsync(string searchquery, int posicion)
        {
            string request = "api/ViewRestaurantes/GetPaginationRestaurantesView?searchquery=" + searchquery + "&posicion=" + posicion;
            return await this.CallApiAsync<PaginationRestaurantesView>(request);
        }

        public async Task<PaginationRestaurantesView> FilterPaginationRestaurantesViewAsync(string categoria, string searchquery, int posicion)
        {
            string request = "api/ViewRestaurantes/FilterPaginationRestaurantesView?categoria=" + categoria + "&searchquery=" + searchquery + "&posicion=" + posicion;
            return await this.CallApiAsync<PaginationRestaurantesView>(request);
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
            string request = "api/ViewRestaurantes/CategoriasProductos/" + idrestaurante;
            return await this.CallApiAsync<List<CategoriaProducto>>(request);
        }

        public async Task<CategoriaProducto> CreateCategoriaProductoAsync(int idrestaurante, string categoria)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/CategoriasProductos/" + idrestaurante + "/" + categoria;
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
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
            string request = "api/Productos/GetProductosByCategoria/" + restaurante + "/" + categoria;
            return await this.CallApiAsync<List<Producto>>(request);
        }

        public async Task<Producto> FindProductoAsync(int id)
        {
            string request = "api/Productos/" + id;
            return await this.CallApiAsync<Producto>(request);
        }

        public async Task<List<Producto>> FindListProductosAsync(IEnumerable<int> ids)
        {
            string request = "api/Productos/FindListProductos" + string.Join(",", ids);
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
                    httpContext.Session.SetString("TOKEN", token);
                    return keys.GetValue("user").ToObject<Usuario>();
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
                RegisterUserAPIModel model = new RegisterUserAPIModel
                {
                    Usuario = user,
                    RawPassword = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent context = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, context);
                return await response.Content.ReadAsAsync<Usuario>();
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
            (int idusuario, int idrestaurante, List<ProductoCesta> cesta)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Pedidos/" + idusuario + "/" + idrestaurante;
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string json = JsonConvert.SerializeObject(cesta);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                return await response.Content.ReadAsAsync<Pedido>();
            }
        }

        public async Task<List<Pedido>> GetPedidosUsuarioAsync(int idusuario)
        {
            string request = "api/Pedidos/GetPedidosUsuario/" + idusuario;
            return await this.CallApiAsync<List<Pedido>>(request);
        }

        public async Task<List<Pedido>> GetPedidosRestauranteAsync(int idusuario)
        {
            string request = "api/Pedidos/GetPedidosRestaurante/" + idusuario;
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
            string request = "api/ProductosPedidoView/" + string.Join(",", idpedidos);
            return await this.CallApiAsync<List<ProductoPedidoView>>(request);
        }
        #endregion

        #region VALORACIONES_RESTAURANTE
        public async Task<ValoracionRestaurante> GetValoracionRestauranteAsync
            (int idrestaurante, int idusuario)
        {
            string request = "api/ValoracionesRestaurante/" + idrestaurante + "/" + idusuario;
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
