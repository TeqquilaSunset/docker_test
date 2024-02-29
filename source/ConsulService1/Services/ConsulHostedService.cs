
using Consul;
using ConsulService1.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;


namespace ConsulService1.Services
{
    public class ConsulHostedService : IHostedService
    {
        private IConsulClient _consulClient;
        private IConfiguration _configuration;
        private readonly int _hostPort;
        private readonly string _idService;

        public ConsulHostedService(IConsulClient consulClient, IConfiguration configuration) 
        {
            _consulClient = consulClient;
            _configuration = configuration;
            _hostPort = GetPort();
            _idService = $"service-api-{GenerateShortUid(8)}-{_hostPort}";
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var registration = CreateAgentServiceRegistration();
            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);

            await GetValueAsync<ConfigFromConsul>(key: "service1");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
           await _consulClient.Agent.ServiceDeregister(_idService);
        }

        private AgentServiceRegistration CreateAgentServiceRegistration()
        {
            return new AgentServiceRegistration
            {
                ID = _idService,
                Name = $"ServiceApi",
                Tags = ["urlprefix-/ServiceApi strip=/ServiceApi"],
                Address = "host.docker.internal",
                Port = _hostPort,
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(5),
                    HTTP = $"http://host.docker.internal:{_hostPort}/healthCheck",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(2)
                },
            };
        }

        public async Task GetValueAsync<T>(string key)
        {
            var getPair = await _consulClient.KV.Get(key);

            if (getPair?.Response == null)
            {
                return;
            }

            var value = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
            var deserializedValue = JsonSerializer.Deserialize<T>(value);

            if (deserializedValue is ConfigFromConsul configFromConusl)
            {
                _configuration["Message"] = configFromConusl.Message;
            }
            return;
        }
        private static string GenerateShortUid(int length)
        {
            Guid guid = Guid.NewGuid();
            string shortUid = guid.ToString().Substring(0, length);
            return shortUid;
        }

        private static int GetPort()
        {
            string url = Environment.GetEnvironmentVariable("ASPNETCORE_URL") ?? "http://localhost:5775";
            var uri = new Uri(url);
            var port = uri.Port;
            return port;
        }

    }
}
