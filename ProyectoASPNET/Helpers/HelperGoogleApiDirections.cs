using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProyectoASPNET.Models;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace ProyectoASPNET.Helpers
{
    public class HelperGoogleApiDirections
    {
        private readonly string _googleApiKey;
        public readonly IHttpClientFactory _httpClientFactory;

        public HelperGoogleApiDirections(string googleApiKey, IHttpClientFactory httpClientFactory)
        {
            _googleApiKey = googleApiKey;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DistanceMatrixInfo> GetDistanceMatrixInfoAsync(string origen, string destino)
        {
            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
                $"?origins={origen}&destinations={destino}&key={_googleApiKey}&region=es&language=es&mode=bicycling";

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            using (var response = await client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    DistanceMatrixResponse distanceMatrix =
                        JsonConvert.DeserializeObject<DistanceMatrixResponse>(content);
                    return new DistanceMatrixInfo
                    {
                        Distancia = distanceMatrix.Rows[0].Elements[0].Distance.Text,
                        TiempoEstimado = distanceMatrix.Rows[0].Elements[0].Duration.Value / 60
                    };
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }


        protected class DistanceMatrixResponse
        {
            public string[] Destination_Addresses { get; set; }
            public string[] Origin_Addresses { get; set; }
            public Row[] Rows { get; set; }
            public string Status { get; set; }
        }

        protected class Row
        {
            public Element[] Elements { get; set; }
        }

        protected class Element
        {
            public Distance Distance { get; set; }
            public Duration Duration { get; set; }
        }

        protected class Distance
        {
            public string Text { get; set; }
            public int Value { get; set; }
        }

        protected class Duration
        {
            public string Text { get; set; }
            public int Value { get; set; }
        }
    }
}

