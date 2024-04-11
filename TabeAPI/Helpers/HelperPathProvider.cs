using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

namespace ProyectoASPNET.Helpers
{
    public enum Folders
    { ImagRestaurantes = 0, ImagProductos = 1 }

    public class HelperPathProvider
    {
        private IWebHostEnvironment hostEnvironment;
        private IServer server;

        public HelperPathProvider
            (IWebHostEnvironment hostEnvironment,
            IServer server)
        {
            this.hostEnvironment = hostEnvironment;
            this.server = server;
        }

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
    }
}
