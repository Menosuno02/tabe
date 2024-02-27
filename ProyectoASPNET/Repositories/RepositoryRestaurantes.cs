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

CREATE OR ALTER VIEW V_USUARIOS
AS
	SELECT U.IDUSUARIO, U.NOMBRE, U.APELLIDOS,
	U.CORREO, U.CONTRASENYA, U.TELEFONO, U.DIRECCION,
	T.NOMBRETIPO
	FROM USUARIOS U
	INNER JOIN TIPOS_USUARIOS T
	ON U.TIPOUSUARIO = T.IDTIPO
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
        var consulta = from datos in this.context.Restaurantes
                       orderby datos.Valoracion descending
                       select datos;
        return await consulta.ToListAsync();
    }

    public async Task<RestauranteView> FindRestaurante(int id)
    {
        var consulta = from datos in this.context.Restaurantes
                       where datos.IdRestaurante == id
                       select datos;
        return await consulta.FirstOrDefaultAsync();
    }

    public async Task<List<RestauranteView>> FilterRestaurantes(string categoria, int rating)
    {
        if (categoria == "Todas")
        {
            var consulta = from datos in this.context.Restaurantes
                           where datos.Valoracion >= rating
                           orderby datos.Valoracion descending
                           select datos;
            return await consulta.ToListAsync();
        }
        else
        {
            var consulta = from datos in this.context.Restaurantes
                           where datos.CategoriaRestaurante == categoria
                            && datos.Valoracion >= rating
                           orderby datos.Valoracion descending
                           select datos;
            return await consulta.ToListAsync();
        }
    }
    #endregion

    public async Task<List<CategoriaRestaurante>> GetCategoriasRestaurantes()
    {
        var consulta = from datos in this.context.CategoriaRestaurantes
                       orderby datos.Nombre
                       select datos;
        return await consulta.ToListAsync();
    }

    public async Task<List<CategoriaProducto>> GetCategoriaProductos()
    {
        var consulta = from datos in this.context.CategoriasProducto
                       select datos;
        return await consulta.ToListAsync();
    }

    public async Task<List<Producto>> GetProductosRestaurante(int restaurante)
    {
        var consulta = from datos in this.context.Productos
                       where datos.IdRestaurante == restaurante
                       select datos;
        return await consulta.ToListAsync();
    }

    public async Task<List<Producto>> GetProductoCategorias(int restaurante, int categoria)
    {
        string sql = "SP_PRODUCTOS_CATEGORIA @RESTAURANTE, @CATEGORIA";
        SqlParameter paramRestaurante = new SqlParameter("@RESTAURANTE", restaurante);
        SqlParameter paramCategoria = new SqlParameter("@CATEGORIA", categoria);
        var consulta = this.context.Productos.FromSqlRaw(sql, paramRestaurante, paramCategoria);
        return await consulta.ToListAsync();
    }

}
