using Microsoft.AspNetCore.Authentication;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json;
using System.Drawing;
using static System.Net.WebRequestMethods;

namespace ConsulService1.Services
{
    /// <summary>
    /// Автоматическая регистрация сервиса при запуске и дерегистрация при выключении в consul 
    /// </summary>
    public class ConsulHttpClientService : IConsulHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrlConsul = $"http://host.docker.internal:8500/v1/agent/service";
        private readonly int _hostPort;
        private readonly string _idService;

        public ConsulHttpClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _hostPort = GetPort();
            _idService = $"service-api-{GenerateShortUid(8)}-{_hostPort}";
        }

        /// <summary>
        /// Регистрация сервиса в consul
        /// </summary>
        public async Task RegisterServiceAsync()
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                

                var serviceDefinition = new
                {
                    ID = _idService, //service-api-{uid}-{port}
                    Name = $"ServiceApi",
                    Tags = new List<string> { "urlprefix-/prediction" },
                    Address = "host.docker.internal",
                    Port = _hostPort,
                    EnableTagOverride = false,
                    Check = new
                    {
                        DeregisterCriticalServiceAfter = "1m",
                        HTTP = $"http://host.docker.internal:{_hostPort}/healthCheck",
                        Interval = "10s"
                    }
                };
                var json = JsonConvert.SerializeObject(serviceDefinition);

                var response = await httpClient.PutAsync(_baseUrlConsul + "/register", new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to register the service in consul, statuscode:{response.StatusCode}");
                }
            }
        }

        /// <summary>
        /// Дерегистрация сервиса в consul
        /// </summary>
        public async Task DeregisterServiceAsync()
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.PutAsync(_baseUrlConsul + $"/deregister/{_idService}", null);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to deregister the service in consul, statuscode:{response.StatusCode}");
                }
            }

        }

        private static string GenerateShortUid(int length)
        {
            Guid guid = Guid.NewGuid();
            string shortUid = guid.ToString().Substring(0, length);
            return shortUid;
        }

        private static int GetPort()
        {
            string url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:5000";
            var uri = new Uri(url);
            var port = uri.Port;
            return port;
        }

    }
}
