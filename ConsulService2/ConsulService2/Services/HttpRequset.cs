using System.Text;

namespace ConsulService2.Services
{
    public class HttpRequset
    {
        HttpClient client = new HttpClient();

        public async Task<string> GetStirngPrediction()
        {
            HttpResponseMessage response = await client.GetAsync("http://host.docker.internal:9999/prediction");
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        
    }
}
