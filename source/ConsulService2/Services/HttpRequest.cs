using System.Net.Http;
using System.Text;

namespace ConsulService2.Services
{
    public class HttpRequest : IHttpRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HttpRequest(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> GetStringPredictionAsync()
        {
            try
            {
                var adressFabio = _configuration["Fabio:Adress"];
                var portFabio = _configuration["Fabio:Port"];
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"http://{adressFabio}:{portFabio}/ServiceApi/prediction");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    throw new Exception($"Request failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while making the request: {ex.Message}");
            }
        }
    }
}
