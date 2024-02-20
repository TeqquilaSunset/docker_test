using System.Net.Http;
using System.Text;

namespace ConsulService2.Services
{
    public class HttpRequest : IHttpReuest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpRequest(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetStringPredictionAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("http://host.docker.internal:9999/prediction");

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
