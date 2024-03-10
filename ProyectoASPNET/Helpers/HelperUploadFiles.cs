namespace ProyectoASPNET.Helpers
{
    public class HelperUploadFiles
    {
        private HelperPathProvider helperPathProvider;

        public HelperUploadFiles(HelperPathProvider helperPathProvider)
        {
            this.helperPathProvider = helperPathProvider;
        }

        public async Task<string> UploadFileAsync(IFormFile file, Folders folder, int id)
        {
            // string fileName = file.FileName;
            string fileName = "img" + id + ".jpeg";
            string path =
                this.helperPathProvider.MapPath(fileName, folder);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // return path;
            return fileName;
        }
    }
}
