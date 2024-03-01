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
                httpContext.Session.SetObject("CESTA", cesta);
            }
        }
    }
}
