using Consul;
using ConsulService2.Models;

namespace ConsulService2.Helpers
{
    public class ConsulGetConfigure
    {
        private IConsulClient _consulClient;
         public  ConsulGetConfigure(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public async Task GetConfigure()
        {
            var consulDemoKey = await ConsulKeyValueProvider.GetValueAsync<ConsulDemoKey>(key: "service1");
            if (consulDemoKey != null)
            {
                Dictionary<string, object?> dict2 = consulDemoKey.GetType().GetProperties().ToDictionary(
                        prop => prop.Name,
                        prop => prop.GetValue(consulDemoKey, null)
                    );
                //builder.Configuration.AddInMemoryCollection(dict2.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value.ToString())));
            }
        }
    }
}
