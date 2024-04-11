using ProyectoASPNET.Extensions;
using ProyectoASPNET.Models;
using ProyectoASPNET.Services;
using System.Security.Claims;

namespace ProyectoASPNET.Helpers
{
    public class HelperCesta
    {
        IHttpContextAccessor httpContextAccessor;
        IServiceRestaurantes service;

        public HelperCesta(IHttpContextAccessor httpContextAccessor, IServiceRestaurantes service)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.service = service;
        }

        public List<ProductoCesta> GetCesta()
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            return httpContext.Session.GetObject
                <List<ProductoCesta>>("CESTA");
        }

        public async Task<CestaView> GetDatosCesta()
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            decimal total = 0;
            List<ProductoCestaView> cestaView = new List<ProductoCestaView>();
            List<ProductoCesta> cesta = GetCesta();
            if (cesta != null)
            {
                IEnumerable<int> ids = cesta.Select(p => p.IdProducto);
                List<Producto> productos =
                    await this.service.FindListProductosAsync(cesta.Select(p => p.IdProducto));
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
            if (httpContext.Session.GetString("CESTA") == null)
            {
                List<ProductoCesta> cesta = new List<ProductoCesta> { prod };
                httpContext.Session.SetObject("CESTA", cesta);
                Producto productoPivot = await this.service.FindProductoAsync(prod.IdProducto);
                httpContext.Session.SetObject("RESTAURANTE", productoPivot.IdRestaurante);
            }
            else
            {
                List<ProductoCesta> cesta = httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA");
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
                httpContext.Session.SetObject("CESTA", cesta);
            }
        }

        public void DeleteProductoCesta(int idproducto)
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext.Session.GetString("CESTA") != null)
            {
                List<ProductoCesta> cesta = httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA");
                cesta.RemoveAll(p => p.IdProducto == idproducto);
                if (cesta.Count() == 0)
                {
                    httpContext.Session.Remove("CESTA");
                    httpContext.Session.Remove("RESTAURANTE");
                }
                else
                {
                    httpContext.Session.SetObject("CESTA", cesta);
                }
            }
        }

        public void UpdateProductoCesta(int idproducto, int cantidad)
        {
            if (cantidad == 0) DeleteProductoCesta(idproducto);
            else
            {
                HttpContext httpContext = this.httpContextAccessor.HttpContext;
                List<ProductoCesta> cesta = httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA");
                cesta
                .FirstOrDefault(p => p.IdProducto == idproducto)
                .Cantidad = cantidad;
                httpContext.Session.SetObject("CESTA", cesta);
            }
        }

        public async Task<Pedido> CreatePedido()
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext.Session.GetString("CESTA") != null)
            {
                List<ProductoCesta> cesta = httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA");
                int idusuario = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                int idrestaurante = httpContext.Session.GetObject<int>("RESTAURANTE");
                Pedido pedido =
                    await this.service.CreatePedidoAsync(idusuario, idrestaurante, cesta);
                httpContext.Session.Remove("CESTA");
                httpContext.Session.Remove("RESTAURANTE");
                return pedido;
            }
            return null;
        }
    }
}
