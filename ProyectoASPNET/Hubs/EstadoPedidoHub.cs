using Microsoft.AspNetCore.SignalR;
using ProyectoASPNET.Services;
using StackExchange.Redis;
using TabeNuget;

namespace ProyectoASPNET.Hubs
{
    public class EstadoPedidoHub : Hub
    {
        private IServiceRestaurantes service;

        public EstadoPedidoHub(IServiceRestaurantes service)
        {
            this.service = service;
        }
    }
}
