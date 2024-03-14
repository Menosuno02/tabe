using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoASPNET.Data;
using ProyectoASPNET.Helpers;
using ProyectoASPNET.Models;

namespace ProyectoASPNET;

#region VIEWS y PROCEDURES
/*
CREATE OR ALTER VIEW V_RESTAURANTES
AS
	SELECT R.IDRESTAURANTE, R.NOMBRE, R.TELEFONO, R.CORREO,
	R.DIRECCION, R.IMAGEN, CR.NOMBRECATEGORIA,
	CAST(ISNULL(AVG(V.VALORACION), 0) AS DECIMAL(4,2)) AS VALORACION
	FROM RESTAURANTES R
	INNER JOIN CATEGORIAS_RESTAURANTES CR
	ON R.IDCATEGORIA = CR.IDCATEGORIA
	LEFT JOIN VALORACIONES_RESTAURANTE V
	ON R.IDRESTAURANTE = V.IDRESTAURANTE
	GROUP BY R.IDRESTAURANTE, R.NOMBRE, R.TELEFONO, R.CORREO,
	R.DIRECCION, R.IMAGEN, CR.NOMBRECATEGORIA
GO

CREATE OR ALTER VIEW V_PRODUCTOS_PEDIDO AS
	SELECT P.IDPEDIDO, PR.IDPRODUCTO, R.NOMBRE AS RESTAURANTE,
	E.NOMBRE AS ESTADO, PR.NOMBRE AS PRODUCTO, PR.PRECIO, PP.CANTIDAD
	FROM PEDIDOS P
	INNER JOIN PRODUCTOS_PEDIDO PP
	ON P.IDPEDIDO = PP.IDPEDIDO
	INNER JOIN RESTAURANTES R
	ON P.IDRESTAURANTE = R.IDRESTAURANTE
	INNER JOIN PRODUCTOS PR
	ON PR.IDPRODUCTO = PP.IDPRODUCTO
	INNER JOIN ESTADOS_PEDIDOS E
	ON P.ESTADO = E.IDESTADO
GO

CREATE OR ALTER PROCEDURE SP_PRODUCTOS_CATEGORIA
(@RESTAURANTE INT, @CATEGORIA INT)
AS
	SELECT P.IDPRODUCTO, P.NOMBRE, P.DESCRIPCION,
	P.PRECIO, P.IMAGEN, P.IDRESTAURANTE
	FROM PRODUCTOS P
	INNER JOIN PRODUCTO_CATEGORIAS PC
	ON P.IDPRODUCTO = PC.IDPRODUCTO
	WHERE PC.IDCATEGORIA = @CATEGORIA
	AND P.IDRESTAURANTE = @RESTAURANTE
GO

CREATE OR ALTER PROCEDURE SP_CREATE_USUARIO
(@NOMBRE NVARCHAR(100), @APELLIDOS NVARCHAR(100),
@CORREO NVARCHAR(100), @CONTRASENYA VARBINARY(MAX),
@TELEFONO NVARCHAR(9), @DIRECCION NVARCHAR(200),
@SALT NVARCHAR(50))
AS
	DECLARE @IDUSUARIO INT
	SELECT @IDUSUARIO = ISNULL(MAX(IDUSUARIO), 0) + 1
	FROM USUARIOS
	INSERT INTO USUARIOS VALUES
	(@IDUSUARIO, @NOMBRE, @APELLIDOS, @CORREO,
	@CONTRASENYA, @SALT, @TELEFONO, @DIRECCION, 1)
GO
*/
#endregion

public class RepositoryRestaurantes
{
    private RestaurantesContext context;
    private HelperGoogleApiDirections helperGoogleApi;
    private HelperUploadFiles helperUploadFiles;

    public RepositoryRestaurantes
        (RestaurantesContext context,
        HelperGoogleApiDirections helperGoogleApi,
        HelperUploadFiles helperUploadFiles)
    {
        this.context = context;
        this.helperGoogleApi = helperGoogleApi;
        this.helperUploadFiles = helperUploadFiles;
    }

    #region RESTAURANTES
    private async Task<int> GetMaxIdRestaurante()
    {
        if (this.context.Restaurantes.Count() == 0) return 1;
        return await this.context.Restaurantes.MaxAsync(r => r.IdRestaurante) + 1;
    }

    public async Task<List<Restaurante>> GetRestaurantesAsync()
    {
        return await this.context.Restaurantes.ToListAsync();
    }

    public async Task<Restaurante> FindRestauranteAsync(int id)
    {
        return await this.context.Restaurantes
            .Where(r => r.IdRestaurante == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Restaurante> CreateRestauranteAsync(Restaurante restaurante, string password, IFormFile imagen)
    {
        restaurante.IdRestaurante = await GetMaxIdRestaurante();
        restaurante.Imagen =
            await helperUploadFiles.UploadFileAsync(imagen, Folders.ImagRestaurantes, restaurante.IdRestaurante);
        restaurante.Direccion = await helperGoogleApi.GetValidatedDireccionAsync(restaurante.Direccion);
        await this.context.Restaurantes.AddAsync(restaurante);
        Usuario usuRestaurante = new Usuario
        {
            Nombre = restaurante.Nombre,
            Correo = restaurante.Correo,
            Telefono = restaurante.Telefono,
            Direccion = restaurante.Direccion,
            TipoUsuario = 3
        };
        await this.RegisterUsuarioAsync(usuRestaurante, password);
        await this.context.SaveChangesAsync();
        return restaurante;
    }

    public async Task EditRestauranteAsync(Restaurante restaurante, IFormFile imagen)
    {
        if (imagen != null)
        {
            restaurante.Imagen =
                await helperUploadFiles.UploadFileAsync(imagen, Folders.ImagRestaurantes, restaurante.IdRestaurante);
        }
        restaurante.Direccion = await helperGoogleApi.GetValidatedDireccionAsync(restaurante.Direccion);
        Restaurante restEditar = await this.FindRestauranteAsync(restaurante.IdRestaurante);
        Usuario usuEditar = await this.GetUsuarioFromRestauranteAsync(restEditar.Correo); // Buscar con correo antiguo
        usuEditar.Telefono = restaurante.Telefono;
        usuEditar.Direccion = restaurante.Direccion;
        usuEditar.Correo = restaurante.Correo;
        restEditar.Nombre = restaurante.Nombre;
        restEditar.Telefono = restaurante.Telefono;
        restEditar.Direccion = restaurante.Direccion;
        restEditar.Imagen = restaurante.Imagen;
        restEditar.CategoriaRestaurante = restaurante.CategoriaRestaurante;
        restEditar.Correo = restaurante.Correo;
        await context.SaveChangesAsync();
    }

    public async Task DeleteRestauranteAsync(int id)
    {
        List<Producto> productos = await this.context.Productos
            .Where(p => p.IdRestaurante == id)
            .ToListAsync();
        foreach (Producto producto in productos)
        {
            this.context.ProductoCategorias.RemoveRange
                (await this.context.ProductoCategorias
                .Where(pc => pc.IdProducto == producto.IdProducto)
                .ToListAsync());
            this.context.ProductoPedidos.RemoveRange
                (await this.context.ProductoPedidos
                .Where(pc => pc.IdProducto == producto.IdProducto)
                .ToListAsync());
        }
        this.context.ValoracionRestaurantes.RemoveRange
            (await this.context.ValoracionRestaurantes
            .Where(v => v.IdRestaurante == id)
            .ToListAsync());
        await this.context.SaveChangesAsync();
        this.context.CategoriasProducto.RemoveRange
            (await this.context.CategoriasProducto
            .Where(cp => cp.IdRestaurante == id)
            .ToListAsync());
        this.context.Productos.RemoveRange(productos);
        this.context.Pedidos.RemoveRange
            (await this.context.Pedidos
            .Where(p => p.IdRestaurante == id)
            .ToListAsync());
        await this.context.SaveChangesAsync();
        Restaurante restaurante = await FindRestauranteAsync(id);
        this.context.Usuarios.Remove
            (await this.context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == restaurante.Correo));
        await this.context.SaveChangesAsync();
        this.context.Restaurantes.Remove(restaurante);
        await this.context.SaveChangesAsync();
    }

    public async Task<Restaurante> GetRestauranteFromLoggedUserAsync(int id)
    {
        Usuario usuario = await this.context.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == id);
        return await this.context.Restaurantes
            .FirstOrDefaultAsync(r => r.Correo == usuario.Correo);
    }

    public async Task<Usuario> GetUsuarioFromRestauranteAsync(string restCorreo)
    {
        return await this.context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == restCorreo);
    }
    #endregion

    #region V_RESTAURANTES
    public async Task<List<RestauranteView>> GetRestaurantesViewAsync(string searchquery)
    {
        List<RestauranteView> restaurantes = await this.context.RestaurantesView
            .OrderByDescending(r => r.Valoracion)
            .ToListAsync();
        if (searchquery != "")
            restaurantes = restaurantes
                .Where(r => r.Nombre
                .Contains(searchquery, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        return restaurantes;
    }

    public async Task<RestauranteView> FindRestauranteViewAsync(int id)
    {
        return await this.context.RestaurantesView
            .Where(r => r.IdRestaurante == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<RestauranteView>> FilterRestaurantesViewAsync(string categoria, string searchquery)
    {
        List<RestauranteView> restaurantes = await this.context.RestaurantesView
            .Where(r => r.CategoriaRestaurante == categoria)
            .OrderByDescending(r => r.Valoracion)
            .ToListAsync();
        if (searchquery != "")
            restaurantes = restaurantes
                .Where(r => r.Nombre
                .Contains(searchquery, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        return restaurantes;
    }
    #endregion

    #region CATEGORIAS_RESTAURANTES
    public async Task<List<CategoriaRestaurante>> GetCategoriasRestaurantesAsync()
    {
        return await this.context.CategoriaRestaurantes
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }
    #endregion

    #region CATEGORIAS_PRODUCTOS
    private async Task<int> GetMaxIdCategoriaProducto()
    {
        if (this.context.CategoriasProducto.Count() == 0) return 1;
        return await this.context.CategoriasProducto.MaxAsync(cp => cp.IdCategoriaProducto) + 1;
    }

    public async Task<List<CategoriaProducto>> GetCategoriasProductosAsync(int idrestaurante)
    {
        return await this.context.CategoriasProducto
            .Where(cp => cp.IdRestaurante == idrestaurante)
            .ToListAsync();
    }

    public async Task<CategoriaProducto> CreateCategoriaProductoAsync(int idrestaurante, string categoria)
    {
        CategoriaProducto categoriaProducto = new CategoriaProducto
        {
            IdCategoriaProducto = await GetMaxIdCategoriaProducto(),
            IdRestaurante = idrestaurante,
            Nombre = categoria
        };
        await this.context.CategoriasProducto.AddAsync(categoriaProducto);
        await this.context.SaveChangesAsync();
        return categoriaProducto;
    }

    public async Task DeleteCategoriaProductoAsync(int idcategoria)
    {
        List<ProductoCategorias> relationsCategoria = await this.context.ProductoCategorias
            .Where(pc => pc.IdCategoria == idcategoria)
            .ToListAsync();
        this.context.ProductoCategorias.RemoveRange(relationsCategoria);
        await this.context.SaveChangesAsync();
        CategoriaProducto categoriaProducto = await this.context.CategoriasProducto
            .FirstOrDefaultAsync(cp => cp.IdCategoriaProducto == idcategoria);
        this.context.CategoriasProducto.Remove(categoriaProducto);
        await this.context.SaveChangesAsync();
    }
    #endregion

    #region PRODUCTO_CATEGORIAS
    public async Task<List<string>> GetCategoriasFromProductoAsync(int idprod)
    {
        List<int> categ = await this.context.ProductoCategorias
            .Where(pc => pc.IdProducto == idprod)
            .Select(pc => pc.IdCategoria)
            .ToListAsync();
        return await this.context.CategoriasProducto
            .Where(cp => categ.Contains(cp.IdCategoriaProducto))
            .Select(cp => cp.Nombre)
            .ToListAsync();
    }
    #endregion

    #region PRODUCTOS
    private async Task<int> GetMaxIdProducto()
    {
        if (this.context.Productos.Count() == 0) return 1;
        return await this.context.Productos.MaxAsync(r => r.IdProducto) + 1;
    }

    public async Task<List<Producto>> GetProductosAsync()
    {
        return await this.context.Productos.ToListAsync();
    }

    public async Task<List<Producto>> GetProductosRestauranteAsync(int id)
    {
        return await this.context.Productos
            .Where(p => p.IdRestaurante == id)
            .ToListAsync();
    }

    public async Task<List<Producto>> GetProductosByCategoriaAsync(int restaurante, int categoria)
    {
        /*
        string sql = "SP_PRODUCTOS_CATEGORIA @RESTAURANTE, @CATEGORIA";
        SqlParameter paramRestaurante = new SqlParameter("@RESTAURANTE", restaurante);
        SqlParameter paramCategoria = new SqlParameter("@CATEGORIA", categoria);
        var consulta = this.context.Productos.FromSqlRaw(sql, paramRestaurante, paramCategoria);
        return await consulta.ToListAsync();
        */
        return await this.context.Productos.Join(this.context.ProductoCategorias
            .Where(pc => pc.IdCategoria == categoria),
            p => p.IdProducto,
            pc => pc.IdProducto,
            (p, pc) => new Producto
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Imagen = p.Imagen,
                IdRestaurante = p.IdRestaurante
            })
            .Where(pc => pc.IdRestaurante == restaurante)
            .ToListAsync();
    }

    public async Task<Producto> FindProductoAsync(int id)
    {
        return await this.context.Productos
            .Where(p => p.IdProducto == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Producto>> FindListProductosAsync(IEnumerable<int> ids)
    {
        return await this.context.Productos
            .Where(p => ids.Contains(p.IdProducto))
            .ToListAsync();
    }

    public async Task<Producto> CreateProductoAsync(Producto producto, int[] categproducto, IFormFile imagen)
    {
        producto.IdProducto = await this.GetMaxIdProducto();
        producto.Imagen =
            await this.helperUploadFiles.UploadFileAsync(imagen, Folders.ImagProductos, producto.IdProducto);
        await this.context.Productos.AddAsync(producto);
        await this.context.SaveChangesAsync();
        foreach (int categoria in categproducto)
        {
            await this.context.ProductoCategorias.AddAsync(new ProductoCategorias
            {
                IdCategoria = categoria,
                IdProducto = producto.IdProducto
            });
        }
        await this.context.SaveChangesAsync();
        return producto;
    }

    public async Task EditProductoAsync(Producto producto, int[] categproducto, IFormFile imagen)
    {
        if (imagen != null)
        {
            producto.Imagen =
                await helperUploadFiles.UploadFileAsync(imagen, Folders.ImagProductos, producto.IdProducto);
        }
        Producto prodEditar = await this.FindProductoAsync(producto.IdProducto);
        prodEditar.Nombre = producto.Nombre;
        prodEditar.Precio = producto.Precio;
        prodEditar.Descripcion = producto.Descripcion;
        prodEditar.Imagen = producto.Imagen;
        List<ProductoCategorias> pc = await this.context.ProductoCategorias
                .Where(pc => pc.IdProducto == producto.IdProducto)
                .ToListAsync();
        foreach (ProductoCategorias categoria in pc)
        {
            this.context.ProductoCategorias.Remove(categoria);
        }
        foreach (int categoria in categproducto)
        {
            await this.context.ProductoCategorias.AddAsync(new ProductoCategorias
            {
                IdCategoria = categoria,
                IdProducto = producto.IdProducto
            });
        }
        await context.SaveChangesAsync();
    }

    public async Task DeleteProductoAsync(int id)
    {
        Producto producto = await FindProductoAsync(id);
        this.context.ProductoCategorias.RemoveRange
            (await this.context.ProductoCategorias
            .Where(pc => pc.IdProducto == id)
            .ToListAsync());
        this.context.ProductoPedidos.RemoveRange
            (await this.context.ProductoPedidos
            .Where(pc => pc.IdProducto == id)
            .ToListAsync());
        await this.context.SaveChangesAsync();
        this.context.Productos.Remove(producto);
        await this.context.SaveChangesAsync();
    }
    #endregion

    #region USUARIOS
    private async Task<int> GetMaxIdUsuarioAsync()
    {
        if (this.context.Usuarios.Count() == 0) return 1;
        return await this.context.Usuarios.MaxAsync(x => x.IdUsuario) + 1;
    }

    public async Task<Usuario> LoginUsuarioAsync(string email, string password)
    {
        Usuario user = await this.context.Usuarios
            .FirstOrDefaultAsync(x => x.Correo == email);
        if (user == null)
            return null;
        else
        {
            string salt = user.Salt;
            byte[] temp =
                HelperCryptography.EncryptPassword(password, salt);
            byte[] passUser = user.Contrasenya;
            bool response = HelperTools.CompareArrays(temp, passUser);
            if (response == true)
                return user;
            else
                return null;
        }
    }

    public async Task<Usuario> RegisterUsuarioAsync(Usuario user, string password)
    {
        user.Direccion = await helperGoogleApi.GetValidatedDireccionAsync(user.Direccion);
        user.IdUsuario = await GetMaxIdUsuarioAsync();
        user.Salt = HelperTools.GenerateSalt();
        user.Contrasenya = HelperCryptography.EncryptPassword
            (password, user.Salt);
        await this.context.Usuarios.AddAsync(user);
        await this.context.SaveChangesAsync();
        return user;
    }

    public async Task<List<Usuario>> GetUsuariosAsync()
    {
        return await this.context.Usuarios.ToListAsync();
    }

    public async Task<Usuario> FindUsuarioAsync(int id)
    {
        return await this.context.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == id);
    }

    public async Task EditUsuarioAsync(Usuario user)
    {
        user.Direccion = await helperGoogleApi.GetValidatedDireccionAsync(user.Direccion);
        Usuario usuEditar = await this.FindUsuarioAsync(user.IdUsuario);
        usuEditar.Nombre = user.Nombre;
        usuEditar.Direccion = user.Direccion;
        usuEditar.Telefono = user.Telefono;
        usuEditar.Correo = user.Correo;
        await context.SaveChangesAsync();
    }
    #endregion

    #region PEDIDOS
    private async Task<int> GetMaxIdPedidoAsync()
    {
        if (this.context.Pedidos.Count() == 0) return 1;
        return await this.context.Pedidos.MaxAsync(x => x.IdPedido) + 1;
    }

    public async Task<Pedido> CreatePedidoAsync
        (int idusuario, int idrestaurante, List<ProductoCesta> cesta)
    {
        Pedido pedido = new Pedido
        {
            IdPedido = await GetMaxIdPedidoAsync(),
            IdUsuario = idusuario,
            IdRestaurante = idrestaurante,
            Estado = 1,
            Fecha = DateTime.Now
        };
        await this.context.Pedidos.AddAsync(pedido);
        foreach (ProductoCesta producto in cesta)
        {
            await this.context.AddAsync(new ProductoPedido
            {
                IdPedido = pedido.IdPedido,
                IdProducto = producto.IdProducto,
                Cantidad = producto.Cantidad
            });
        }
        await this.context.SaveChangesAsync();
        return pedido;
    }

    public async Task<List<Pedido>> GetPedidosUsuarioAsync(int idusuario)
    {
        // Solo pedidos sin entregar
        return await this.context.Pedidos
            .Where(p => p.IdUsuario == idusuario && p.Estado != 4)
            .ToListAsync();
    }

    public async Task<List<Pedido>> GetPedidosRestauranteAsync(int idusuario)
    {
        Usuario usuario = await this.context.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == idusuario);
        Restaurante rest = await this.context.Restaurantes
            .FirstOrDefaultAsync(r => r.Correo == usuario.Correo);
        return await this.context.Pedidos
            .Where(p => p.IdRestaurante == rest.IdRestaurante)
            .ToListAsync();
    }
    #endregion

    #region ESTADOS_PEDIDO
    public async Task<List<EstadoPedido>> GetEstadoPedidosAsync()
    {
        return await this.context.EstadoPedidos.ToListAsync();
    }

    public async Task UpdateEstadoPedidoAsync(int idpedido, int estado)
    {
        Pedido pedido = await this.context.Pedidos
            .FirstOrDefaultAsync(p => p.IdPedido == idpedido);
        pedido.Estado = estado;
        await this.context.SaveChangesAsync();
    }
    #endregion

    #region V_PRODUCTOS_PEDIDO
    public async Task<List<ProductoPedidoView>> GetProductosPedidoViewAsync(List<int> idpedidos)
    {
        return await this.context.ProductosPedidoView
            .Where(p => idpedidos.Contains(p.IdPedido))
            .ToListAsync();
    }
    #endregion

    #region VALORACIONES_RESTAURANTE
    public async Task<ValoracionRestaurante> GetValoracionRestauranteAsync
        (int idrestaurante, int idusuario)
    {
        return await this.context.ValoracionRestaurantes
            .Where(v => v.IdRestaurante == idrestaurante && v.IdUsuario == idusuario)
            .FirstOrDefaultAsync();
    }
    public async Task UpdateValoracionRestauranteAsync(ValoracionRestaurante val)
    {
        ValoracionRestaurante valBorrar =
            await this.context.ValoracionRestaurantes
            .Where(v => v.IdRestaurante == val.IdRestaurante
                && v.IdUsuario == val.IdUsuario)
            .FirstOrDefaultAsync();
        if (valBorrar != null)
        {
            this.context.ValoracionRestaurantes.Remove(valBorrar);
        }
        await this.context.ValoracionRestaurantes.AddAsync(val);
        await this.context.SaveChangesAsync();
    }
    #endregion
}
