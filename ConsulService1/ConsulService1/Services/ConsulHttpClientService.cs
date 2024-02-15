using Microsoft.AspNetCore.Authentication;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json;
using System.Drawing;

namespace ConsulService1.Services
{
    public class ConsulHttpClientService : IConsulHttpClient
    {
        private const string _baseUrlConsul = $"http://host.docker.internal:8500/v1/agent";
        private HttpClient _http = new HttpClient();
        private string _baseName = $"service-api-{GenerateShortUid(8)}";
        private int index = 1;

        /// <summary>
        /// Регистрация сервиса в conmsul
        /// </summary>
        /// <returns></returns>
        public async Task RegisterServiceAsync()
        {
            
            var serviceDefinition = new 
            {
                ID = _baseName, //придумать над id
                Name = "ServiceApi",
                Tags = new List<string> { "urlprefix-/prediction" },
                Address = "host.docker.internal",
                Port = 5277, //подумать над портами
                EnableTagOverride = false,
                Check = new
                {
                    DeregisterCriticalServiceAfter = "3m",
                    HTTP = "http://host.docker.internal:5277/healthCheck",
                    Interval = "10s"
                }
            };
            var json = JsonConvert.SerializeObject(serviceDefinition);

            var response = await _http.PutAsync(_baseUrlConsul + "/service/register", new StringContent(json, Encoding.UTF8, "application/json"));

            if(!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to register the service in consul, statuscode:{response.StatusCode}");
            }
            
        }

        /// <summary>
        /// Дерегистрация сервиса в consul
        /// </summary>
        /// <returns></returns>
        public async Task DeregisterServiceAsync()
        {
            var response = await _http.PutAsync(_baseUrlConsul + $"/service/deregister/{_baseName}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to deregister the service in consul, statuscode:{response.StatusCode}");
            }
        }

        private static string GenerateShortUid(int length)
        {
            Guid guid = Guid.NewGuid();
            string shortUid = guid.ToString().Substring(0, length);
            return shortUid;
        }

    }
}
