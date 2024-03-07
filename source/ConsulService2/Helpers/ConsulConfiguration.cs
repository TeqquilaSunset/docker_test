using Consul;
using ConsulService2.Models;
using System.Text;
using System.Text.Json;

namespace ConsulService2.Helpers
{
    public class ConsulConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly string _urlConsul;

        public ConsulConfiguration(IConfiguration configuration, string urlConsul)
        {
            _configuration = configuration;
            _urlConsul = urlConsul;
        }

        public void Configure()
        {
            var consulClient = new ConsulClient(configuration =>
            {
                configuration.Address = new Uri(_urlConsul);
            });

            ConfigureRabbit(consulClient);
            ConfigureFabio(consulClient);
        }

        private void ConfigureRabbit(ConsulClient consulClient)
        {
            var getPairRabbit = consulClient.KV.Get("rabbit").Result;
            if (getPairRabbit?.Response != null)
            {
                var value = Encoding.UTF8.GetString(getPairRabbit.Response.Value, 0, getPairRabbit.Response.Value.Length);
                var consulConfig2 = JsonSerializer.Deserialize<RabbitConfig>(value);

                _configuration["Rabbit:Password"] = consulConfig2.Password;
                _configuration["Rabbit:User"] = consulConfig2.User;
                _configuration["Rabbit:Url"] = consulConfig2.Url;
            }
        }

        private void ConfigureFabio(ConsulClient consulClient)
        {
            var getPairFabio = consulClient.KV.Get("fabio").Result;
            if (getPairFabio?.Response != null)
            {
                var value = Encoding.UTF8.GetString(getPairFabio.Response.Value, 0, getPairFabio.Response.Value.Length);
                var consulConfig3 = JsonSerializer.Deserialize<FabioConfig>(value);

                _configuration["Fabio:Adress"] = consulConfig3.Adress;
                _configuration["Fabio:Port"] = consulConfig3.Port.ToString();
            }
        }
    }
}
