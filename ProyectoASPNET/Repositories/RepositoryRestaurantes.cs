using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoASPNET.Data;
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
@CORREO NVARCHAR(100), @CONTRASENYA NVARCHAR(100),
@TELEFONO NVARCHAR(9), @DIRECCION NVARCHAR(200))
AS
	DECLARE @IDUSUARIO INT
	SELECT @IDUSUARIO = MAX(IDUSUARIO) + 1
	FROM USUARIOS
	INSERT INTO USUARIOS VALUES
	(@IDUSUARIO, @NOMBRE, @APELLIDOS, @CORREO,
	@CONTRASENYA, @TELEFONO, @DIRECCION, 1)
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
    public async Task<List<RestauranteView>> GetRestaurantes()
    {
        var consulta = from datos in this.context.RestaurantesView
                       orderby datos.Valoracion descending
                       select datos;
        return await consulta.ToListAsync();
    }

    public async Task<RestauranteView> FindRestaurante(int id)
    {
        var consulta = from datos in this.context.RestaurantesView
                       where datos.IdRestaurante == id
                       select datos;
        return await consulta.FirstOrDefaultAsync();
    }

    public async Task<List<RestauranteView>> FilterRestaurantes(string categoria, int rating)
    {
        var consulta = from datos in this.context.RestaurantesView
                       where (datos.Valoracion >= rating && categoria == "Todas") ||
                       (datos.Valoracion >= rating && datos.CategoriaRestaurante == categoria)
                       orderby datos.Valoracion descending
                       select datos;
        return await consulta.ToListAsync();
    }
    #endregion

    #region CATEGORIAS_RESTAURANTES
    public async Task<List<CategoriaRestaurante>> GetCategoriasRestaurantes()
    {
        var consulta = from datos in this.context.CategoriaRestaurantes
                       orderby datos.Nombre
                       select datos;
        return await consulta.ToListAsync();
    }
    #endregion

    #region CATEGORIAS_PRODUCTOS
    public async Task<List<CategoriaProducto>> GetCategoriaProductos()
    {
        var consulta = from datos in this.context.CategoriasProducto
                       select datos;
        return await consulta.ToListAsync();
    }
    #endregion

    #region PRODUCTOS
    public async Task<List<Producto>> GetProductosRestaurante(int restaurante)
    {
        var consulta = from datos in this.context.Productos
                       where datos.IdRestaurante == restaurante
                       select datos;
        return await this.context.Productos
            .Where(datos => datos.IdRestaurante == restaurante)
            .ToListAsync();
    }

    // Productos según la categoría
    public async Task<List<Producto>> GetProductoCategorias(int restaurante, int categoria)
    {
        string sql = "SP_PRODUCTOS_CATEGORIA @RESTAURANTE, @CATEGORIA";
        SqlParameter paramRestaurante = new SqlParameter("@RESTAURANTE", restaurante);
        SqlParameter paramCategoria = new SqlParameter("@CATEGORIA", categoria);
        var consulta = this.context.Productos.FromSqlRaw(sql, paramRestaurante, paramCategoria);
        return await consulta.ToListAsync();
    }
    #endregion

    #region USUARIOS
    public async Task<Usuario> LoginUsuario(string email, string password)
    {
        var consulta = from datos in this.context.Usuarios
                       where datos.Correo == email && datos.Contrasenya == password
                       select datos;
        return await consulta.FirstOrDefaultAsync();
    }

    public async Task RegisterUsuario(Usuario usuario)
    {
        string sql = "SP_CREATE_USUARIO @NOMBRE, @APELLIDOS, @CORREO, @CONTRASENYA, @TELEFONO, @DIRECCION";
        SqlParameter paramNombre = new SqlParameter("@NOMBRE", usuario.Nombre);
        SqlParameter paramApellidos = new SqlParameter("@APELLIDOS", usuario.Apellidos);
        SqlParameter paramCorreo = new SqlParameter("@CORREO", usuario.Correo);
        SqlParameter paramContrasenya = new SqlParameter("@CONTRASENYA", usuario.Contrasenya);
        SqlParameter paramTelefono = new SqlParameter("@TELEFONO", usuario.Telefono);
        SqlParameter paramDireccion = new SqlParameter("@DIRECCION", usuario.Direccion);
        await this.context.Database
            .ExecuteSqlRawAsync(sql, paramNombre, paramApellidos, paramCorreo, paramContrasenya, paramTelefono, paramDireccion);
    }
    #endregion

}
