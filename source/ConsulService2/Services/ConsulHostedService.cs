using Consul;
using ConsulService2.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ConsulService2.Services
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly int _hostPort;
        private readonly string _idService;

        public ConsulHostedService(IConsulClient consulClient)
        {
            _consulClient = consulClient;
            _hostPort = GetPort();
            _idService = $"service-front-{GenerateShortUid(8)}-{_hostPort}";
        }

        /// <summary>
        /// Запускается при старте
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var registration = CreateAgentServiceRegistration();
            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
        }

        /// <summary>
        /// Запускается при завершении
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _consulClient.Agent.ServiceDeregister(_idService);
        }

        private AgentServiceRegistration CreateAgentServiceRegistration()
        {
            return new AgentServiceRegistration
            {
                ID = _idService,
                Name = $"ServiceFront",
                Tags = ["urlprefix-/ServiceFront strip=/ServiceFront"],
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
