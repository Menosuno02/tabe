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
	SELECT R.IDRESTAURANTE, R.NOMBRE, R.TELEFONO,
	R.DIRECCION, R.IMAGEN, R.TIEMPOENTREGA, CR.NOMBRECATEGORIA,
	CAST(ISNULL(AVG(V.VALORACION), 0) AS DECIMAL(4,2)) AS VALORACION
	FROM RESTAURANTES R
	INNER JOIN CATEGORIAS_RESTAURANTES CR
	ON R.IDCATEGORIA = CR.IDCATEGORIA
	LEFT JOIN VALORACIONES_RESTAURANTE V
	ON R.IDRESTAURANTE = V.IDRESTAURANTE
	GROUP BY R.IDRESTAURANTE, R.NOMBRE, R.TELEFONO,
	R.DIRECCION, R.IMAGEN, R.TIEMPOENTREGA, CR.NOMBRECATEGORIA
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

    public RepositoryRestaurantes(RestaurantesContext context)
    {
        this.context = context;
    }

    #region RESTAURANTES
    private async Task<int> GetMaxIdRestaurante()
    {
        if (this.context.Restaurantes.Count() == 0) return 1;
        return await this.context.Restaurantes.MaxAsync(r => r.IdRestaurante) + 1;
    }

    public async Task<Restaurante> FindRestauranteAsync(int id)
    {
        return await this.context.Restaurantes
            .Where(r => r.IdRestaurante == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Restaurante> CreateRestauranteAsync(Restaurante restaurante)
    {
        restaurante.IdRestaurante = await GetMaxIdRestaurante();
        await this.context.Restaurantes.AddAsync(restaurante);
        await this.context.SaveChangesAsync();
        return restaurante;
    }

    public async Task EditRestauranteAsync(Restaurante restaurante)
    {
        Restaurante restEditar = await this.FindRestauranteAsync(restaurante.IdRestaurante);
        restEditar.Nombre = restaurante.Nombre;
        restEditar.Telefono = restaurante.Telefono;
        restEditar.Direccion = restaurante.Direccion;
        restEditar.Imagen = restaurante.Imagen;
        restEditar.CategoriaRestaurante = restaurante.CategoriaRestaurante;
        restEditar.TiempoEntrega = restaurante.TiempoEntrega;
        await context.SaveChangesAsync();
    }

    public async Task DeleteRestauranteAsync(int id)
    {
        Restaurante restaurante = await FindRestauranteAsync(id);
        this.context.Restaurantes.Remove(restaurante);
        await this.context.SaveChangesAsync();
    }
    #endregion

    #region V_RESTAURANTES
    public async Task<List<RestauranteView>> GetRestaurantesViewAsync()
    {
        return await this.context.RestaurantesView
            .OrderByDescending(r => r.Valoracion)
            .ToListAsync();
    }

    public async Task<RestauranteView> FindRestauranteViewAsync(int id)
    {
        return await this.context.RestaurantesView
            .Where(r => r.IdRestaurante == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<RestauranteView>> FilterRestaurantesViewAsync(string categoria, int rating)
    {
        return await this.context.RestaurantesView
            .Where(r => r.Valoracion >= rating && categoria == "Todas" ||
                       r.Valoracion >= rating && r.CategoriaRestaurante == categoria)
            .OrderByDescending(r => r.Valoracion)
            .ToListAsync();
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
    public async Task<List<CategoriaProducto>> GetCategoriasProductosAsync()
    {
        return await this.context.CategoriasProducto.ToListAsync();
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

    public async Task<Producto> CreateProductoAsync(Producto producto)
    {
        await this.context.Productos.AddAsync(producto);
        await this.context.SaveChangesAsync();
        return producto;
    }

    public async Task EditProductoAsync(Producto producto, int[] categproducto)
    {
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

    public async Task<Usuario> RegisterUsuarioAsync(string password, Usuario user)
    {
        user.IdUsuario = await GetMaxIdUsuarioAsync();
        user.Salt = HelperTools.GenerateSalt();
        user.Contrasenya = HelperCryptography.EncryptPassword
            (password, user.Salt);
        await this.context.Usuarios.AddAsync(user);
        await this.context.SaveChangesAsync();
        return user;
        /*
        string sql = "SP_CREATE_USUARIO @NOMBRE, @APELLIDOS, @CORREO, @CONTRASENYA, @TELEFONO, @DIRECCION, @SALT";
        SqlParameter paramNombre = new SqlParameter("@NOMBRE", user.Nombre);
        SqlParameter paramApellidos = new SqlParameter("@APELLIDOS", user.Apellidos);
        SqlParameter paramCorreo = new SqlParameter("@CORREO", user.Correo);
        SqlParameter paramContrasenya = new SqlParameter("@CONTRASENYA", user.Contrasenya);
        SqlParameter paramTelefono = new SqlParameter("@TELEFONO", user.Telefono);
        SqlParameter paramDireccion = new SqlParameter("@DIRECCION", user.Direccion);
        SqlParameter paramSalt = new SqlParameter("@SALT", user.Salt);
        await this.context.Database
            .ExecuteSqlRawAsync(sql, paramNombre, paramApellidos, paramCorreo,
            paramContrasenya, paramTelefono, paramDireccion, paramSalt);
        */
    }

    public async Task<List<Usuario>> GetUsuariosAsync()
    {
        return await this.context.Usuarios.ToListAsync();
    }

    public async Task<Usuario> FindUsuarioAsync(int id)
    {
        return await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
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
    #endregion

    #region V_PRODUCTOS_PEDIDO
    public async Task<List<ProductoPedidoView>> GetProductosPedidoViewAsync(List<int> idpedidos)
    {
        return await this.context.ProductosPedidoView
            .Where(p => idpedidos.Contains(p.IdPedido))
            .ToListAsync();
    }
    #endregion
}
