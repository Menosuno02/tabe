using Microsoft.AspNetCore.SignalR;
using ProyectoASPNET.Services;

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
