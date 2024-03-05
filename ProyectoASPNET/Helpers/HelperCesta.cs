using ProyectoASPNET.Extensions;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Helpers
{
    public class HelperCesta
    {
        IHttpContextAccessor httpContextAccessor;
        RepositoryRestaurantes repo;

        public HelperCesta(IHttpContextAccessor httpContextAccessor, RepositoryRestaurantes repo)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.repo = repo;
        }

        public List<ProductoCesta> GetCesta()
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            return httpContext.Session.GetObject
                <List<ProductoCesta>>("CESTA");
        }

        public async Task UpdateCesta(ProductoCesta prod)
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA") == null)
            {
                List<ProductoCesta> cesta = new List<ProductoCesta> { prod };
                httpContext.Session.SetObject("CESTA", cesta);
                Producto productoPivot = await this.repo.FindProductoAsync(prod.IdProducto);
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
            if (httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA") != null)
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

        public async Task CreatePedido()
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA") != null)
            {
                List<ProductoCesta> cesta = httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA");
                int user = httpContext.Session.GetObject<int>("USER");
                int idrestaurante = httpContext.Session.GetObject<int>("RESTAURANTE");
                Pedido pedido = await this.repo.CreatePedidoAsync(user, idrestaurante);
                foreach (ProductoCesta producto in cesta)
                {
                    await this.repo.CreateProductoPedidoAsync(pedido.IdPedido, producto.IdProducto, producto.Cantidad);
                }
                httpContext.Session.Remove("CESTA");
                httpContext.Session.Remove("RESTAURANTE");
            }
        }
    }
}
