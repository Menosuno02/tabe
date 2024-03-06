﻿using ProyectoASPNET.Extensions;
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
                    await this.repo.FindListProductosAsync(cesta.Select(p => p.IdProducto));
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
            int id = httpContext.Session.GetObject<int>("USER");
            Usuario usuario = await this.repo.FindUsuarioAsync(id);
            return new CestaView
            {
                Cesta = cestaView,
                Total = total,
                Nombre = usuario.Nombre + " " + usuario.Apellidos,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono
            };
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
                Pedido pedido =
                    await this.repo.CreatePedidoAsync(user, idrestaurante, cesta);
                httpContext.Session.Remove("CESTA");
                httpContext.Session.Remove("RESTAURANTE");
            }
        }
    }
}
