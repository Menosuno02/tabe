using ProyectoASPNET.Extensions;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Helpers
{
    public class HelperCesta
    {
        IHttpContextAccessor httpContextAccessor;

        public HelperCesta(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public List<ProductoCesta> GetCesta()
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            return httpContext.Session.GetObject
                <List<ProductoCesta>>("CESTA");

        }

        public void UpdateCesta(ProductoCesta prod)
        {
            HttpContext httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA") == null)
            {
                List<ProductoCesta> cesta = new List<ProductoCesta> { prod };
                httpContext.Session.SetObject("CESTA", cesta);
            }
            else
            {
                List<ProductoCesta> cesta = httpContext.Session.GetObject
                    <List<ProductoCesta>>("CESTA");
                if (cesta
                    .Where(prod => prod.IdProducto == prod.IdProducto)
                    .Count() == 0)
                {
                    cesta.Add(prod);
                }
                else
                {
                    cesta
                        .FirstOrDefault(prod => prod.IdProducto == prod.IdProducto)
                        .Cantidad += prod.Cantidad;
                    int cantidad = cesta
                        .FirstOrDefault(prod => prod.IdProducto == prod.IdProducto)
                        .Cantidad;
                    decimal precio = cantidad * prod.Precio;
                    cesta
                       .FirstOrDefault(prod => prod.IdProducto == prod.IdProducto)
                       .Precio = precio;
                    precio = cesta
                       .FirstOrDefault(prod => prod.IdProducto == prod.IdProducto)
                       .Precio;
                }
                httpContext.Session.SetObject("CESTA", cesta);
            }
        }
    }
}
