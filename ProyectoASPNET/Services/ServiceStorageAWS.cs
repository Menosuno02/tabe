using Amazon.S3.Model;
using Amazon.S3;
using TabeNuget;

namespace ProyectoASPNET.Services
{
    public class ServiceStorageAWS
    {
        private IAmazonS3 client;
        private string BucketName;

        public ServiceStorageAWS(KeysModel keys, IAmazonS3 client)
        {
            this.BucketName = keys.BucketName;
            this.client = client;
        }

        public async Task<bool> UploadFileAsync(string fileName, Stream stream)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                InputStream = stream,
                Key = fileName,
                BucketName = this.BucketName
            };
            PutObjectResponse response = await this.client.PutObjectAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            DeleteObjectResponse response = await this.client.DeleteObjectAsync(this.BucketName, fileName);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
