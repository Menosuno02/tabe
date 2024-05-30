using Newtonsoft.Json;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Helpers;
using TabeNuget;
using StackExchange.Redis;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;

namespace ProyectoASPNET.Services
{
    public class ServiceCacheRedis
    {
        private IHttpContextAccessor httpContextAccessor;
        private IServiceRestaurantes service;
        private IDistributedCache cache;

        public ServiceCacheRedis(IDistributedCache cache, IHttpContextAccessor httpContextAccessor, IServiceRestaurantes service)
        {
            this.cache = cache;
            this.httpContextAccessor = httpContextAccessor;
            this.service = service;
        }

        public async Task<List<ProductoCesta>> GetCesta()
        {

            HttpContext httpContext = this.httpContextAccessor.HttpContext;


            int id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);


            string json = await this.cache.GetStringAsync("cesta" + id);

            if (json == null)

                return null;

            else

                return JsonConvert.DeserializeObject<List<ProductoCesta>>(json);

        }

        public async Task<CestaView> GetDatosCesta()
        {

            HttpContext httpContext = this.httpContextAccessor.HttpContext;

            decimal total = 0;
            List<ProductoCestaView> cestaView = new List<ProductoCestaView>();
            List<ProductoCesta> cesta = await GetCesta();
            if (cesta != null)
            {
                List<int> ids = cesta.Select(p => p.IdProducto).ToList();
                List<Producto> productos =
                    await this.service.FindListProductosAsync(ids);
                foreach (Producto producto in productos)
                {

                    int cantidad = cesta
                        .FirstOrDefault(p => p.IdProducto == producto.IdProducto)
                        .Cantidad;

                    cestaView.Add(new ProductoCestaView
                    {
                        IdProducto = producto.IdProducto,
                        Nombre = producto.Nombre,
                        Precio = producto.Precio * cantidad,
                        Cantidad = cantidad,
                        Imagen = producto.Imagen
                    });
                    total += producto.Precio * cantidad;
                }
            }

            int id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Usuario usuario = await this.service.FindUsuarioAsync(id);
            return new CestaView
            {
                Cesta = cestaView,
                Total = total,
                Nombre = usuario.Nombre,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono
            };
        }

        public async Task UpdateCesta(ProductoCesta prod)
        {

            HttpContext httpContext = this.httpContextAccessor.HttpContext;


            int id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (await GetCesta() == null)
            {
                List<ProductoCesta> cesta = new List<ProductoCesta> { prod };
                string json = JsonConvert.SerializeObject(cesta);
                await this.cache.SetStringAsync("cesta" + id, json);
                Producto productoPivot = await this.service.FindProductoAsync(prod.IdProducto);
                json = JsonConvert.SerializeObject(productoPivot.IdRestaurante);
                await this.cache.SetStringAsync("restaurante" + id, json);
            }
            else
            {
                List<ProductoCesta> cesta = await GetCesta();
                if (cesta
                    .Where(p => p.IdProducto == prod.IdProducto)
                    .Count() == 0)
                {
                    cesta.Add(prod);
                }
                else
                {

                    cesta
                        .FirstOrDefault(p => p.IdProducto == prod.IdProducto)
                        .Cantidad += prod.Cantidad;

                }
                string json = JsonConvert.SerializeObject(cesta);
                await this.cache.SetStringAsync("cesta" + id, json);
            }
        }

        public async Task DeleteProductoCesta(int idproducto)
        {
            if (await GetCesta() != null)
            {

                HttpContext httpContext = this.httpContextAccessor.HttpContext;


                int id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                List<ProductoCesta> cesta = await GetCesta();
                cesta.RemoveAll(p => p.IdProducto == idproducto);
                if (cesta.Count() == 0)
                    await ResetCesta();
                else
                {
                    string json = JsonConvert.SerializeObject(cesta);
                    await this.cache.SetStringAsync("cesta" + id, json);
                }
            }
        }

        public async Task UpdateProductoCesta(int idproducto, int cantidad)
        {
            if (cantidad == 0)
                await DeleteProductoCesta(idproducto);
            else
            {

                HttpContext httpContext = this.httpContextAccessor.HttpContext;


                int id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                List<ProductoCesta> cesta = await GetCesta();

                cesta
                    .FirstOrDefault(p => p.IdProducto == idproducto)
                    .Cantidad = cantidad;

                string json = JsonConvert.SerializeObject(cesta);
                await this.cache.SetStringAsync("cesta" + id, json);
            }
        }

        public async Task<Pedido> CreatePedido()
        {
            if (await GetCesta() != null)
            {

                HttpContext httpContext = this.httpContextAccessor.HttpContext;


                int id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                List<ProductoCesta> cesta = await GetCesta();

                string json = await this.cache.GetStringAsync("restaurante" + id);

#pragma warning disable CS8604 // Possible null reference argument.
                int idrestaurante = JsonConvert.DeserializeObject<int>(json);
#pragma warning restore CS8604 // Possible null reference argument.
                Pedido pedido =
                    await this.service.CreatePedidoAsync(idrestaurante, cesta);
                await ResetCesta();
                return pedido;
            }

            return null;

        }

        public async Task ResetCesta()
        {

            HttpContext httpContext = this.httpContextAccessor.HttpContext;


            int id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await this.cache.RemoveAsync("cesta" + id);
            await this.cache.RemoveAsync("restaurante" + id);
        }
    }
}
