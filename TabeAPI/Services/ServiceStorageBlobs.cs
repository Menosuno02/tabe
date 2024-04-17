using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using ProyectoASPNET.Models;

namespace TabeAPI.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            BlobContainerClient containerClient =
                client.GetBlobContainerClient(containerName);
            List<BlobModel> models = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobModel blob = new BlobModel
                {
                    Nombre = item.Name,
                    Contenedor = containerName,
                    Url = blobClient.Uri.AbsoluteUri
                };
                models.Add(blob);
            }
            return models;
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }
    }
}
