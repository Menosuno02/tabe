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
	ISNULL(AVG(V.VALORACION), 0) AS VALORACION
	FROM RESTAURANTES R
	INNER JOIN CATEGORIAS_RESTAURANTES CR
	ON R.IDCATEGORIA = CR.IDCATEGORIA
	LEFT JOIN VALORACIONES_RESTAURANTE V
	ON R.IDRESTAURANTE = V.IDRESTAURANTE
	GROUP BY R.IDRESTAURANTE, R.NOMBRE, R.TELEFONO,
	R.DIRECCION, R.IMAGEN, R.TIEMPOENTREGA, CR.NOMBRECATEGORIA
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
    public async Task<List<RestauranteView>> GetRestaurantesAsync()
    {
        return await this.context.RestaurantesView
            .OrderByDescending(r => r.Valoracion)
            .ToListAsync();
    }

    public async Task<RestauranteView> FindRestauranteAsync(int id)
    {
        return await this.context.RestaurantesView
            .Where(r => r.IdRestaurante == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<RestauranteView>> FilterRestaurantesAsync(string categoria, int rating)
    {
        return await this.context.RestaurantesView
            .Where(r => r.Valoracion >= rating && categoria == "Todas" ||
                       r.Valoracion >= rating && r.CategoriaRestaurante == categoria)
            .OrderByDescending(r => r.Valoracion)
            .ToListAsync();
    }

    public async Task<RestauranteView> CreateRestauranteViewAsync(RestauranteView restaurante)
    {
        await this.context.RestaurantesView.AddAsync(restaurante);
        await this.context.SaveChangesAsync();
        return restaurante;
    }

    public async Task DeleteRestauranteAsync(int id)
    {
        RestauranteView restaurante = await FindRestauranteAsync(id);
        this.context.RestaurantesView.Remove(restaurante);
        await this.context.SaveChangesAsync();
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
    public async Task<List<CategoriaProducto>> GetCategoriaProductosAsync()
    {
        return await this.context.CategoriasProducto.ToListAsync();
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

    public async Task<Producto> CreateProductoAsync(Producto producto)
    {
        await this.context.Productos.AddAsync(producto);
        await this.context.SaveChangesAsync();
        return producto;
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

    public async Task<Usuario> FindUsuarioAsync(int id)
    {
        return await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
    }
    #endregion
}
