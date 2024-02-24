using System.Data;
using Microsoft.EntityFrameworkCore;
using ProyectoASPNET.Data;
using ProyectoASPNET.Models;

namespace ProyectoASPNET;

#region VIEWS Y PROCEDURES

/*
*/

#endregion

public class RepositoryRestaurantes
{
    private RestaurantesContext context;

    public RepositoryRestaurantes(RestaurantesContext context)
    {
        this.context = context;
    }

    public async Task<List<Restaurante>> GetRestaurantes()
    {
        var consulta = from datos in this.context.Restaurantes
                       select datos;
        return await consulta.ToListAsync();
    }

    public async Task<Restaurante> FindRestaurante(int id)
    {
        var consulta = from datos in this.context.Restaurantes
                       where datos.IdRestaurante == id
                       select datos;
        return await consulta.FirstOrDefaultAsync();
    }
}
