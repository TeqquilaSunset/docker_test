
using Consul;
using ConsulService2.Helpers;
using ConsulService2.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ConsulService2.Services
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _hostPort;
        private readonly string _idService;

        public ConsulHostedService(IConsulClient consulClient, IConfiguration configuration, IServiceProvider serviceProvider) 
        {
            _consulClient = consulClient;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _hostPort = GetPort();
            _idService = $"service-front-{GenerateShortUid(8)}-{_hostPort}";
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var registration = new AgentServiceRegistration
            {

                ID = _idService,
                Name = $"ServiceFront",
                Tags = ["urlprefix-/ServiceFront", "strip-/ServiceFront"],
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

            var consulDemoKey = await ConsulKeyValueProvider.GetValueAsync<ConsulDemoKey>(key: "service1");
            if (consulDemoKey != null)
            {
                Dictionary<string, object?> dict2 = consulDemoKey.GetType().GetProperties().ToDictionary(
                        prop => prop.Name,
                        prop => prop.GetValue(consulDemoKey, null)
                    );
                foreach (var kv in dict2)
                {
                    _configuration[kv.Key] = kv.Value?.ToString();
                }
            }

            var services = _consulClient.Agent.Services().Result.Response;
            var rabbiturl = "";
            foreach (var service in services)
            {
                if (service.Value.Service == "rabbitmq")
                {
                    rabbiturl = service.Value.Address;
                }
            }

            //_serviceProvider.GetRequiredService<IServiceCollection>().AddMassTransit(x =>
            //{
            //    x.UsingRabbitMq((context, cfg) =>
            //    {
            //        cfg.Host(rabbiturl, c =>
            //        {
            //            c.Username("rmuser");
            //            c.Password("rmpassword");
            //        });

            //        cfg.ClearSerialization();
            //        cfg.UseRawJsonSerializer();
            //        cfg.ConfigureEndpoints(context);
            //    });
            //});
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
