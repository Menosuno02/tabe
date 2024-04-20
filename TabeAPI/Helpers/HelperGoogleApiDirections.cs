using Newtonsoft.Json;
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

        public async Task<string> GetValidatedDireccionAsync(string direccion)
        {
            string url = $"https://content-addressvalidation.googleapis.com/v1:validateAddress" +
                $"?alt=json&key={_googleApiKey}";
            var requestBody = new
            {
                address = new
                {
                    addressLines = new string[] { direccion },
                    regionCode = "ES"
                }
            };
            var stringContent =
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = stringContent;
            using (var response = await client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ValidateAddressRoot validateAddressRoot =
                        JsonConvert.DeserializeObject<ValidateAddressRoot>(content);
                    return validateAddressRoot.result.address.formattedAddress;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        protected class ValidateAddressRoot
        {
            public Result result { get; set; }
            public string responseId { get; set; }
        }

        protected class Result
        {
            public Verdict verdict { get; set; }
            public Address address { get; set; }
            public Geocode geocode { get; set; }
            public Metadata metadata { get; set; }
        }

        protected class Verdict
        {
            public string inputGranularity { get; set; }
            public string validationGranularity { get; set; }
            public string geocodeGranularity { get; set; }
            public bool addressComplete { get; set; }
            public bool hasInferredComponents { get; set; }
        }

        protected class Address
        {
            public string formattedAddress { get; set; }
            public Postaladdress postalAddress { get; set; }
            public Addresscomponent[] addressComponents { get; set; }
        }

        protected class Postaladdress
        {
            public string regionCode { get; set; }
            public string languageCode { get; set; }
            public string postalCode { get; set; }
            public string administrativeArea { get; set; }
            public string locality { get; set; }
            public string[] addressLines { get; set; }
        }

        protected class Addresscomponent
        {
            public Componentname componentName { get; set; }
            public string componentType { get; set; }
            public string confirmationLevel { get; set; }
            public bool inferred { get; set; }
        }

        protected class Componentname
        {
            public string text { get; set; }
            public string languageCode { get; set; }
        }

        protected class Geocode
        {
            public Location location { get; set; }
            public Pluscode plusCode { get; set; }
            public Bounds bounds { get; set; }
            public string placeId { get; set; }
            public string[] placeTypes { get; set; }
        }

        protected class Location
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
        }

        protected class Pluscode
        {
            public string globalCode { get; set; }
        }

        protected class Bounds
        {
            public Location low { get; set; }
            public Location high { get; set; }
        }

        protected class Metadata
        {
            public bool business { get; set; }
            public bool residential { get; set; }
        }

    }
}

