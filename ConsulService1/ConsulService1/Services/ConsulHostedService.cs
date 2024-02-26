
using Consul;


namespace ConsulService1.Services
{
    public class ConsulHostedService : IHostedService
    {
        private IConsulClient _consulClient;
        private readonly int _hostPort;
        private readonly string _idService;

        public ConsulHostedService(IConsulClient consulClient) 
        {
            _consulClient = consulClient;
            _hostPort = GetPort();
            _idService = $"service-api-{GenerateShortUid(8)}-{_hostPort}";
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var registration = new AgentServiceRegistration
            {
                ID = _idService,
                Name = $"ServiceApi",
                Tags = ["urlprefix-/prediction"],
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

            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
           await _consulClient.Agent.ServiceDeregister(_idService);
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
