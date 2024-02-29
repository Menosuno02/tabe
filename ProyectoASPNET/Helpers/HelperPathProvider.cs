using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

namespace ProyectoASPNET.Helpers
{
    public enum Folders
    { ImagRestaurantes = 0, ImagProductos = 1 }

    public class HelperPathProvider
    {
        // Necesitamos acceder al sistema de archivos del Web Server (wwwroot)
        private IWebHostEnvironment hostEnvironment;
        private IServer server;

        public HelperPathProvider(IWebHostEnvironment hostEnvironment,
            IServer server)
        {
            this.hostEnvironment = hostEnvironment;
            this.server = server;
        }

        // Creamos private que nos devuelva el nombre de la carpeta
        // dependiendo del folder
        private string GetFolderPath(Folders folder)
        {
            string carpeta = "";
            if (folder == Folders.ImagRestaurantes)
            {
                carpeta = "images/imag_restaurantes";
            }
            else if (folder == Folders.ImagProductos)
            {
                carpeta = "images/imag_productos";
            }
            return carpeta;
        }

        public string MapPath(string fileName, Folders folder)
        {
            string carpeta = GetFolderPath(folder);
            string rootPath = this.hostEnvironment.WebRootPath;
            string path = Path.Combine(rootPath, carpeta, fileName);
            return path;
        }

        public string MapUrlPath(string fileName, Folders folder)
        {
            string carpeta = GetFolderPath(folder);
            var addresses =
                server.Features.Get<IServerAddressesFeature>().Addresses;
            return addresses.FirstOrDefault() + "/" + carpeta + "/" + fileName;
        }
    }
}
