using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ProyectoASPNET.Services
{
    public class ServiceLogicApps
    {
        private MediaTypeWithQualityHeaderValue header;

        public ServiceLogicApps()
        {
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task SendMailAsync(string email, string asunto, string mensaje)
        {
            string urlLogicApp = "https://prod-244.westeurope.logic.azure.com:443/workflows/aac0601d46ea4d2f9f1606651fd5ad9e/triggers/When_a_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_a_HTTP_request_is_received%2Frun&sv=1.0&sig=PanrBH7yqWTFBVdZSTs7z5mzWT2ZkEbmYmRF714RjZM";
            var model = new
            {
                email = email,
                asunto = asunto,
                mensaje = mensaje
            };
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(urlLogicApp, content);
            }
        }
    }

}
